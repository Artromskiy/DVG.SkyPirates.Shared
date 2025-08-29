using DVG.Core;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Services.CommandExecutors;
using DVG.SkyPirates.Shared.Systems;
using DVG.SkyPirates.Shared.Systems.HistorySystems;
using SimpleInjector;
using System;

namespace DVG.SkyPirates.Shared.DI
{
    public static class SharedRegistration
    {
        public static void Register(Container container)
        {
            var commandExecutors = new Type[]
            {
                typeof(SpawnSquadCommandExecutor),
                typeof(SpawnUnitCommandExecutor),
                typeof(FixationCommandExecutor),
                typeof(DirectionCommandExecutor)
                //typeof(CommandLogger)
            };
            container.Register<ICommandExecutorService, CommandExecutorService>(Lifestyle.Singleton);
            container.Collection.Register<ICommandExecutor>(commandExecutors, Lifestyle.Singleton);

            var tickableExecutors = new Type[]
            {
                typeof(TargetSearchSystem),
                typeof(SquadMoveSystem),
                typeof(SquadUnitsSystem),
                typeof(SetTargetSystem),
                typeof(SetTargetDestinationSystem),
                typeof(MoveSystem),
                typeof(SeparationSystem),
                typeof(SimpleBehaviourSystem),
                typeof(BeginAttackSystem),

                typeof(ImpactSystem),
                typeof(AutoHealSystem),
                typeof(DamageSystem),
                typeof(MarkDeadSystem),
                typeof(DeadSquadUnitsSystem),

                typeof(SaveHistorySystem)
            };
            container.Register<ITickableExecutorService, TickableExecutorService>(Lifestyle.Singleton);
            container.Collection.Register<ITickableExecutor>(tickableExecutors, Lifestyle.Singleton);

            var preTickableExecutors = new Type[]
            {
                typeof(ApplyHistorySystem),
            };
            container.Register<IPreTickableExecutorService, PreTickableExecutorService>(Lifestyle.Singleton);
            container.Collection.Register<IPreTickableExecutor>(preTickableExecutors, Lifestyle.Singleton);

            container.Register<IPostTickableExecutorService, PostTickableExecutorService>(Lifestyle.Singleton);

            container.Register<ITargetSearchSystem, TargetSearchSystem>(Lifestyle.Singleton);
            container.Register<ITimelineService, TimelineService>(Lifestyle.Singleton);
        }
    }
}
