using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SimpleHeightSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, CachePosition, Radius, Collide>().NotDisposing().NotDisabled();

        private readonly World _world;

        public SimpleHeightSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var hexMap = _world.FirstOrDefault<HexMap>();
            if (hexMap.Data == null)
                return;

            var query = new SolveCollsionQuery(hexMap);
            _world.InlineQuery<SolveCollsionQuery, Position>(_desc, ref query);
        }

        private readonly struct SolveCollsionQuery : IForEach<Position>
        {
            private readonly HexMap _hexMap;

            public SolveCollsionQuery(HexMap hexMap)
            {
                _hexMap = hexMap;
            }

            public void Update(ref Position position)
            {
                var axial = Hex.WorldToAxial(position);
                bool zero = _hexMap.Data.ContainsKey(axial);
                var up = new int3(0, 1, 0);
                bool p1 = _hexMap.Data.ContainsKey(axial + up);
                bool p2 = _hexMap.Data.ContainsKey(axial + up * 2);
                bool p3 = _hexMap.Data.ContainsKey(axial + up * 3);
                bool m1 = _hexMap.Data.ContainsKey(axial - up);

                if (zero && !p1 && !p2)
                {
                    position.Value.y = Hex.AxialToWorldY(axial.y);
                    return;
                }
                else if (p1 && !p2 && !p3)
                {
                    position.Value.y = Hex.AxialToWorldY(axial.y + 1);
                    return;
                }
                else if (m1 && !zero && !p1)
                {
                    position.Value.y = Hex.AxialToWorldY(axial.y - 1);
                    return;
                }

                Debug.Assert(false, "Wrong height behaviour detected");
            }
        }
    }
}
