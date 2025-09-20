using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Components.Special;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct HistoryArch
    {
        public static void EnsureHistory(World world, Entity entity)
        {
            var action = new EnsureHistoryAction(world, entity);
            HistoryIds.ForEachData(ref action);
        }

        private readonly struct EnsureHistoryAction : IStructGenericAction
        {
            private readonly Entity _entity;
            private readonly World _world;

            public EnsureHistoryAction(World world, Entity entity)
            {
                _entity = entity;
                _world = world;
            }

            public readonly void Invoke<T>() where T : struct
            {
                if (_world.Has<T>(_entity) && !_world.Has<History<T>>(_entity))
                    _world.Add(_entity, History<T>.Create());
            }
        }
    }
}
