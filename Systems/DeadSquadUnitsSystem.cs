using Arch.Core;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    [Obsolete]
    public sealed class DeadSquadUnitsSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Squad>();
        private readonly HashSet<int> _unitsToRemove = new();

        private readonly IEntityFactory _entityFactory;
        private readonly World _world;
        public DeadSquadUnitsSystem(World world, IEntityFactory entityFactory)
        {
            _world = world;
            _entityFactory = entityFactory;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadUnitsQuery(_world, _entityFactory, _unitsToRemove);
            _world.InlineQuery<SquadUnitsQuery, Squad>(_desc, ref query);
        }

        private readonly struct SquadUnitsQuery : IForEach<Squad>
        {
            private readonly World _world;
            private readonly IEntityFactory _entityFactory;
            private readonly HashSet<int> _unitsToRemove;

            public SquadUnitsQuery(World world, IEntityFactory entityFactory, HashSet<int> unitsToRemove)
            {
                _unitsToRemove = unitsToRemove;
                _entityFactory = entityFactory;
                _world = world;
            }

            public readonly void Update(ref Squad squad)
            {
                _unitsToRemove.Clear();
                //foreach (int unit in squad.Units)
                //    if (!_world.IsAlive(_entityFactory.Get(unit)))
                //        _unitsToRemove.Add(unit);

                //if (_unitsToRemove.Count > 0)
                //{
                //    squad.Units = new List<int>(squad.Units);
                //    squad.Units.RemoveAll(_unitsToRemove.Contains);
                //}
            }
        }
    }
}