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
        private QueryDescription _hexDesc = new QueryDescription().WithAll<Position, HexTile>();
        private QueryDescription _desc = new QueryDescription().WithAll<Position, CachePosition, CircleShape>();

        private readonly List<(fix2, fix2, fix2)> _segments = new List<(fix2, fix2, fix2)>();

        private readonly World _world;

        public SolveCollisionSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _segments.Clear();
            var collectQuery = new CollectHexTilesQuery(_segments);
            _world.InlineQuery<CollectHexTilesQuery, Position>(_hexDesc, ref collectQuery);
            var query = new SolveCollsionQuery(_segments.ToArray());
            _world.InlineQuery<SolveCollsionQuery, Position, CachePosition, CircleShape>(_desc, ref query);
        }

        private readonly struct CollectHexTilesQuery : IForEach<Position>
        {
            private readonly List<(fix2 a, fix2 b, fix2 normal)> _segments;

            public CollectHexTilesQuery(List<(fix2 a, fix2 b, fix2 normal)> segments)
            {
                _segments = segments;
            }

            public void Update(ref Position position)
            {
                Hex.WorldToAxial(position.Value.xz);

                var points = Hex.GetHexPoints();
                var normals = Hex.GetHexNormals();

                var center = position.Value.xz;
                for (int j = 0; j < points.Length; j++)
                {
                    var point = center + points[j];
                    var nextPoint = center + points[(j + 1) % points.Length];
                    _segments.Add((point, nextPoint, normals[j]));
                }
            }
        }

        private readonly struct SolveCollsionQuery : IForEach<Position, CachePosition, CircleShape>
        {
            private readonly (fix2, fix2, fix2)[] _segments;

            public SolveCollsionQuery((fix2, fix2, fix2)[] segments)
            {
                _segments = segments;
            }

            public void Update(ref Position position, ref CachePosition cachePosition, ref CircleShape circleShape)
            {
                position.Value.xz = Spatial.SolveCircleMove(_segments,
                    cachePosition.Value.xz, position.Value.xz, circleShape.Radius);
            }
        }
    }
}
