using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class TestAddRemoveSystem : ITickableExecutor
    {
        private readonly QueryDescription _addDesc = new QueryDescription().WithAll<UnitId>().WithNone<AddCompTest>();
        private readonly QueryDescription _removeDesc = new QueryDescription().WithAll<UnitId, AddCompTest>();
        private readonly World _world;
        private readonly List<Entity> _cacheList = new List<Entity>();

        public TestAddRemoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _cacheList.Clear();
            var query = new SelectQuery(_cacheList);
            bool toAdd = tick % 2 == 0;
            if(toAdd)
            {
                _world.InlineQuery(_addDesc, ref query);
            }
            else
            {
                _world.InlineQuery(_removeDesc, ref query);
            }

            if (toAdd)
            {
                foreach (var item in _cacheList)
                {
                    _world.Add<AddCompTest>(item);
                }
            }
            else
            {
                foreach (var item in _cacheList)
                {
                    _world.Remove<AddCompTest>(item);
                }
            }
        }

        private readonly struct SelectQuery : IForEach
        {
            private readonly List<Entity> _units;

            public SelectQuery(List<Entity> deadUnits)
            {
                _units = deadUnits;
            }

            public readonly void Update(Entity entity)
            {
                _units.Add(entity);
            }
        }
    }
}
