using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using DVG.SkyPirates.Shared.Configs;

namespace DVG.SkyPirates.Shared.Systems
{
    public class HexMapCollisionSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Position, CachePosition, CircleShape>();

        private readonly List<(fix2 s, fix2 e, fix2 normal)> _segmentsCache = new List<(fix2, fix2, fix2)>();

        private readonly HexMap _hexMap;
        private readonly World _world;

        public HexMapCollisionSystem(HexMap hexMap, World world)
        {
            _hexMap = hexMap;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SolveCollsionQuery(_hexMap, _segmentsCache);
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
                FindSegments(position.Value.xz);
                position.Value = Spatial.SolveCircleMove(_segmentsCache,
                    cachePosition.Value.xz, position.Value.xz, circleShape.Radius).x_y;
            }

            private void FindSegments(fix2 from)
            {
                _segmentsCache.Clear();
                var axialFrom = Hex.WorldToAxial(from);
                foreach (var item in Hex.AxialNear)
                {
                    var offsetted = item + axialFrom;
                    var floor = offsetted.x_y;

                    var up = floor;
                    up.y = 1;

                    var down = floor;
                    down.y = -1;

                    if (!_hexMap.Map.ContainsKey(up) &&
                        !_hexMap.Map.ContainsKey(down) &&
                        _hexMap.Map.ContainsKey(floor))
                    {
                        continue;
                    }

                    var worldFloor = Hex.AxialToWorld(offsetted);
                    for (int i = 0; i < Hex.HexPoints.Length; i++)
                    {
                        var s = worldFloor + Hex.HexPoints[i];
                        var e = worldFloor + Hex.HexPoints[(i + 1) % Hex.HexPoints.Length];
                        _segmentsCache.Add((s, e, Hex.HexNormals[i]));
                    }
                }
            }
        }
    }
}
