using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class DeadSquadUnitsSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Squad>();

        private readonly HashSet<Entity> _unitsToRemove = new HashSet<Entity>();

        private readonly World _world;
        public DeadSquadUnitsSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _unitsToRemove.Clear();
            var query = new SquadUnitsQuery(_unitsToRemove);
            _world.InlineQuery<SquadUnitsQuery, Squad>(_desc, ref query);
        }

        private readonly struct SquadUnitsQuery : IForEach<Squad>
        {
            private readonly HashSet<Entity> _unitsToRemove;

            public SquadUnitsQuery(HashSet<Entity> unitsToRemove)
            {
                _unitsToRemove = unitsToRemove;
            }

            public readonly void Update(ref Squad squad)
            {
                for (int i = 0; i < squad.units.Count; i++)
                {
                    var unit = squad.units[i];
                    if (unit.Has<Dead>())
                    {
                        _unitsToRemove.Add(unit);
                    }
                }

                if (_unitsToRemove.Count > 0)
                {
                    squad.units = new List<Entity>(squad.units);
                    squad.units.RemoveAll(_unitsToRemove.Contains);
                }
            }
        }
    }
}