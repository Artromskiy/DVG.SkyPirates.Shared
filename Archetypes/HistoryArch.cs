using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct HistoryArch
    {
        public static void ForEachData<T>(T action) where T : IGenericAction
        {
            action.Invoke<Direction>();
            action.Invoke<Fixation>();
            action.Invoke<Position>();
            action.Invoke<Rotation>();
            action.Invoke<Health>();
            action.Invoke<Squad>();
            action.Invoke<Team>();
            action.Invoke<Unit>();
        }

        public static QueryDescription Query<T>()
        {
            return new QueryDescription().WithAll<T, History<T>>();
        }

        public static void EnsureHistory(Entity entity)
        {
            ForEachData(new EnsureHistoryAction(entity));
        }

        private readonly struct EnsureHistoryAction : IGenericAction
        {
            private readonly Entity _entity;

            public EnsureHistoryAction(Entity entity)
            {
                _entity = entity;
            }

            public readonly void Invoke<T>()
            {
                if (_entity.Has<T>() && !_entity.Has<History<T>>())
                    _entity.Add(History<T>.Create());
            }
        }
    }
}
