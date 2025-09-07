using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SolveCollisionSystem : ITickableExecutor
    {
        private const int PartitionSize = 2;
        private QueryDescription _hexDesc = new QueryDescription().WithAll<Position, HexTile>();
        private QueryDescription _desc = new QueryDescription().WithAll<Position, CachePosition, CircleShape>();

        private readonly List<(fix2 s, fix2 e, fix2 normal)> _segmentsCache = new List<(fix2, fix2, fix2)>();

        private readonly Dictionary<int2, List<(fix2 s, fix2 e, fix2 normal)>> _segmentsPartitioning = new Dictionary<int2, List<(fix2 s, fix2 e, fix2 normal)>>();

        private readonly fix2[] _hexPoints = Hex.GetHexPoints();
        private readonly fix2[] _hexNormals = Hex.GetHexNormals();

        private readonly World _world;

        public SolveCollisionSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var collectQuery = new CollectHexTilesQuery(_hexPoints, _hexNormals, _segmentsPartitioning);
            _world.InlineQuery<CollectHexTilesQuery, Position>(_hexDesc, ref collectQuery);
            var query = new SolveCollsionQuery(_segmentsCache, _segmentsPartitioning);
            _world.InlineQuery<SolveCollsionQuery, Position, CachePosition, CircleShape>(_desc, ref query);
        }

        private readonly struct CollectHexTilesQuery : IForEach<Position>
        {
            private readonly fix2[] _hexPoints;
            private readonly fix2[] _hexNormals;
            private readonly Dictionary<int2, List<(fix2 s, fix2 e, fix2 normal)>> _segmentsPartitioning;

            public CollectHexTilesQuery(fix2[] hexPoints, fix2[] hexNormals,
                Dictionary<int2, List<(fix2 s, fix2 e, fix2 normal)>> segmentsPartitioning)
            {
                _hexPoints = hexPoints;
                _hexNormals = hexNormals;
                _segmentsPartitioning = segmentsPartitioning;
                foreach (var item in _segmentsPartitioning)
                    item.Value.Clear();
            }

            public void Update(ref Position position)
            {
                Hex.WorldToAxial(position.Value.xz);

                var center = position.Value.xz;
                for (int j = 0; j < _hexPoints.Length; j++)
                {
                    var s = center + _hexPoints[j];
                    var e = center + _hexPoints[(j + 1) % _hexPoints.Length];
                    var normal = _hexNormals[j];
                    var min = GetQuantizedSquare(fix2.Min(s, e));
                    var max = GetQuantizedSquare(fix2.Max(s, e));

                    for (int y = min.y; y <= max.y; y++)
                    {
                        for (int x = min.x; x <= max.x; x++)
                        {
                            var p = new int2(x, y);
                            if (!_segmentsPartitioning.TryGetValue(p, out var quad))
                                _segmentsPartitioning[p] = quad = new List<(fix2 a, fix2 b, fix2 normal)>();
                            quad.Add((s, e, normal));
                        }
                    }
                }
            }
        }

        private readonly struct SolveCollsionQuery : IForEach<Position, CachePosition, CircleShape>
        {
            private readonly List<(fix2 s, fix2 e, fix2 normal)> _segmentsCache;
            private readonly Dictionary<int2, List<(fix2 s, fix2 e, fix2 normal)>> _segmentsPartitioning;

            public SolveCollsionQuery(List<(fix2 s, fix2 e, fix2 normal)> segmentsCache, Dictionary<int2, List<(fix2 s, fix2 e, fix2 normal)>> segmentsPartitioning)
            {
                _segmentsCache = segmentsCache;
                _segmentsPartitioning = segmentsPartitioning;
            }

            public void Update(ref Position position, ref CachePosition cachePosition, ref CircleShape circleShape)
            {
                FindSegments(position.Value.xz, cachePosition.Value.xz, circleShape.Radius);
                if (Spatial.CircleCast(_segmentsCache,
                    cachePosition.Value.xz, position.Value.xz, circleShape.Radius, out var res))
                {
                    var newPos = res.intersection + res.normal * new fix(1024);
                    if (!Spatial.CircleCast(_segmentsCache,
                        cachePosition.Value.xz, newPos, circleShape.Radius, out _))
                    {
                        position.Value = newPos.x_y;
                    }
                    else
                    {
                        position.Value = cachePosition.Value.x_y;
                    }
                }
                //position.Value = Spatial.SolveCircleMove(_segmentsCache,
                //    cachePosition.Value.xz, position.Value.xz, circleShape.Radius).x_y;
            }

            private List<(fix2 s, fix2 e, fix2 normal)> FindSegments(fix2 pos1, fix2 pos2, fix radius)
            {
                _segmentsCache.Clear();

                var min = GetQuantizedSquare(fix2.Min(pos1, pos2) - radius);
                var max = GetQuantizedSquare(fix2.Max(pos1, pos2) - radius);

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        var p = new int2(x, y);
                        if (!_segmentsPartitioning.TryGetValue(p, out var quad))
                            continue;
                        _segmentsCache.AddRange(quad);
                    }
                }
                return _segmentsCache;
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / PartitionSize, (int)position.y / PartitionSize);
        }
    }
}
