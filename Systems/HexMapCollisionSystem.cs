using Arch.Core;
using DVG.Physics;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class HexMapCollisionSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, CachePosition, Radius, Collide>().NotDisposing().NotDisabled();

        public static event Action<Segment[], fix2, fix2, fix> OnFailedToSolve;
        private readonly ThreadLocal<List<Segment>> _segmentsCache = new(() => new List<Segment>());
        private readonly Dictionary<int3, bool> _walkabilityCache = new();
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

            var query = new SolveCollsionQuery(hexMap, _segmentsCache, _walkabilityCache);
            _world.InlineQuery<SolveCollsionQuery, Position, CachePosition, Radius>(_desc, ref query);
        }

        private readonly struct SolveCollsionQuery : IForEach<Position, CachePosition, Radius>
        {
            private readonly HexMap _hexMap;
            private readonly ThreadLocal<List<Segment>> _segmentsCache;
            private readonly Dictionary<int3, bool> _walkabilityCache;

            public SolveCollsionQuery(HexMap hexMap, ThreadLocal<List<Segment>> segmentsCache, Dictionary<int3, bool> walkabilityCache)
            {
                _hexMap = hexMap;
                _segmentsCache = segmentsCache;
                _walkabilityCache = walkabilityCache;
            }

            public void Update(ref Position position, ref CachePosition cachePosition, ref Radius radius)
            {
                FindSegments(cachePosition);
                Solvers.Segments = _segmentsCache.Value.ToArray();
                fix2 solvedPos = fix2.zero;
                bool failed = false;
                try
                {
                    solvedPos = Solvers.CircleSlide(cachePosition.Value.xz, position.Value.xz - cachePosition.Value.xz, radius);
                }
                catch { failed = true; }
                position.Value.xz = solvedPos;
                failed |= !Walkable(Hex.WorldToAxial(position.Value));
                if (failed)
                {
                    position.Value = cachePosition;
                    OnFailedToSolve?.Invoke(Solvers.Segments.ToArray(), cachePosition.Value.xz, position.Value.xz, radius);
                }
                //Trace.Assert(!failed, "Failed to solve collision");
            }

            private void FindSegments(fix3 from)
            {
                _segmentsCache.Value.Clear();
                var axialFrom = Hex.WorldToAxial(from);

                foreach (var item in Hex.AxialNear)
                {
                    var offsetted = item.x_y + axialFrom;

                    if (Walkable(offsetted))
                    {
                        continue;
                    }

                    var worldFloor = Hex.AxialToWorld(offsetted.xz);
                    for (int i = 0; i < Hex.Points.Length; i++)
                    {
                        var s = worldFloor + Hex.Points[i];
                        var e = worldFloor + Hex.Points[(i + 1) % Hex.Points.Length];
                        _segmentsCache.Value.Add(new(s, e));
                    }
                }
            }

            private bool Walkable(int3 axial)
            {
                if (_walkabilityCache.TryGetValue(axial, out var walkable))
                    return walkable;

                bool zero = _hexMap.Data.ContainsKey(axial);
                var up = new int3(0, 1, 0);
                bool p1 = _hexMap.Data.ContainsKey(axial + up);
                bool p2 = _hexMap.Data.ContainsKey(axial + up * 2);
                bool p3 = _hexMap.Data.ContainsKey(axial + up * 3);
                bool m1 = _hexMap.Data.ContainsKey(axial - up);
                return _walkabilityCache[axial] =
                    (zero && !p1 && !p2) ||
                    (p1 && !p2 && !p3) ||
                    (m1 && !zero && !p1);
            }
        }
    }
}
