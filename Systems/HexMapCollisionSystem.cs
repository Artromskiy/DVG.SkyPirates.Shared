using Arch.Core;
using DVG.Core;
using DVG.Core.Physics;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;
using System.Collections.Generic;
using System.Threading;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class HexMapCollisionSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, CachePosition, Radius>().NotDisposing();

        //private readonly List<Segment> _segmentsCache = new();
        private readonly ThreadLocal<List<Segment>> _segmentsCache = new(() => new List<Segment>());
        private readonly World _world;

        public HexMapCollisionSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var hexMap = _world.FirstOrDefault<HexMap>();
            if (hexMap.Data == null)
                return;

            // TODO multithread this thing
            var query = new SolveCollsionQuery(hexMap, _segmentsCache);
            _world.InlineParallelQuery<SolveCollsionQuery, Position, CachePosition, Radius>(_desc, ref query);
        }

        private readonly struct SolveCollsionQuery : IForEach<Position, CachePosition, Radius>
        {
            private readonly HexMap _hexMap;
            //private readonly List<Segment> _segmentsCache;
            private readonly ThreadLocal<List<Segment>> _segmentsCache;

            public SolveCollsionQuery(HexMap hexMap, ThreadLocal<List<Segment>> segmentsCache)
            {
                _hexMap = hexMap;
                _segmentsCache = segmentsCache;
            }

            public void Update(ref Position position, ref CachePosition cachePosition, ref Radius radius)
            {
                FindSegments(cachePosition.Value.xz);

                var solvedPos = Spatial.SolveCircleMove(_segmentsCache.Value,
                    cachePosition.Value.xz, position.Value.xz, radius.Value).x_y;
                position.Value = solvedPos;
            }

            private void FindSegments(fix2 from)
            {
                _segmentsCache.Value.Clear();
                var axialFrom = Hex.WorldToAxial(from);

                foreach (var item in Hex.AxialNear)
                {
                    var offsetted = item + axialFrom;

                    if (Walkable(offsetted))
                    {
                        continue;
                    }

                    var worldFloor = Hex.AxialToWorld(offsetted);
                    for (int i = 0; i < Hex.Points.Length; i++)
                    {
                        var s = worldFloor + Hex.Points[i];
                        var e = worldFloor + Hex.Points[(i + 1) % Hex.Points.Length];
                        _segmentsCache.Value.Add(new(s, e, Hex.Normals[i]));
                    }
                }
            }

            private bool Walkable(int2 axial)
            {
                var floor = axial.x_y;

                var up = floor;
                up.y = 1;

                var down = floor;
                down.y = -1;

                return
                    !_hexMap.Data.ContainsKey(up) &&
                    !_hexMap.Data.ContainsKey(down) &&
                    _hexMap.Data.ContainsKey(floor);
            }
        }
    }
}
