using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Components.Special;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct HistoryArch
    {
        public static void EnsureHistory(Entity entity)
        {
            entity.AddOrGet<TickInfo>();
            HistoryIds.ForEachData(new EnsureHistoryAction(entity));
        }

        private readonly struct EnsureHistoryAction : IStructGenericAction
        {
            private readonly Entity _entity;

            public EnsureHistoryAction(Entity entity)
            {
                _entity = entity;
            }

            public readonly void Invoke<T>() where T : struct
            {
                if (_entity.Has<T>() && !_entity.Has<History<T>>())
                    _entity.Add(History<T>.Create());
            }
        }
    }
}
