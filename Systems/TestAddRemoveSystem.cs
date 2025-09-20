using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class TestAddRemoveSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithNone<Temp>();
        private readonly World _world;

        public TestAddRemoveSystem(QueryDescription desc, World world)
        {
            _desc = desc;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            if (tick % 2 == 0)
                _world.Add<AddCompTest>(_desc);
            else
                _world.Remove<AddCompTest>(_desc);
        }
    }
}
