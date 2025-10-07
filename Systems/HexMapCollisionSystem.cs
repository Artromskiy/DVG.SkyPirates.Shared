using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class HexMapCollisionSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Position, CachePosition, CircleShape>();

        private readonly List<(fix2 s, fix2 e, fix2 normal)> _segmentsCache = new List<(fix2, fix2, fix2)>();

        private readonly World _world;

        public HexMapCollisionSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.TryGetArchetype(Component<HexMap>.Signature, out var archetype);
            var hexMap = archetype.GetChunk(0).GetFirst<HexMap>();

            var query = new SolveCollsionQuery(hexMap, _segmentsCache);
            _world.InlineQuery<SolveCollsionQuery, Position, CachePosition, CircleShape>(_desc, ref query);
        }

        private readonly struct SolveCollsionQuery : IForEach<Position, CachePosition, CircleShape>
        {
            private readonly HexMap _hexMap;
            private readonly List<(fix2 s, fix2 e, fix2 normal)> _segmentsCache;

            public SolveCollsionQuery(HexMap hexMap,
                List<(fix2, fix2, fix2)> segmentsCache)
            {
                _hexMap = hexMap;
                _segmentsCache = segmentsCache;
            }

            public void Update(ref Position position, ref CachePosition cachePosition, ref CircleShape circleShape)
            {
                FindSegments(cachePosition.Value.xz);

                var solvedPos = Spatial.SolveCircleMove(_segmentsCache,
                    cachePosition.Value.xz, position.Value.xz, circleShape.Radius).x_y;
                position.Value = solvedPos;
            }

            private void FindSegments(fix2 from)
            {
                _segmentsCache.Clear();
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
                        _segmentsCache.Add((s, e, Hex.Normals[i]));
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
                    !_hexMap.Map.ContainsKey(up) &&
                    !_hexMap.Map.ContainsKey(down) &&
                    _hexMap.Map.ContainsKey(floor);
            }
        }
    }
}
