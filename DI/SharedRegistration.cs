using Arch.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Services.CommandExecutors;
using DVG.SkyPirates.Shared.Systems;
using SimpleInjector;
using System;

namespace DVG.SkyPirates.Shared.DI
{
    public static class SharedRegistration
    {
        public static void Register(Container container)
        {
            container.RegisterSingleton(() =>
            {
                var world = World.Create();
                world.Create<Temp>(100000);
                world.Remove<Temp>(new QueryDescription().WithAll<Temp>());
                return world;
            });

            var commandExecutors = new Type[]
            {
                typeof(SpawnSquadCommandExecutor),
                typeof(SpawnUnitCommandExecutor),
                typeof(FixationCommandExecutor),
                typeof(DirectionCommandExecutor)
                //typeof(CommandLogger)
            };

            container.RegisterSingleton<IUnitConfigFactory, UnitConfigFactory>();
            container.RegisterSingleton<ISquadFactory, SquadFactory>();
            container.RegisterSingleton<IUnitFactory, UnitFactory>();

            container.RegisterSingleton<ICommandExecutorService, CommandExecutorService>();
            container.Collection.Register<ICommandExecutor>(commandExecutors, Lifestyle.Singleton);

            var tickableExecutors = new Type[]
            {
                typeof(CachePositionSystem),
                typeof(TargetSearchSystem),
                typeof(SquadMoveSystem),
                typeof(SquadUnitsSystem),
                typeof(SetTargetSystem),
                typeof(SetTargetDestinationSystem),
                typeof(MoveSystem),
                typeof(SeparationSystem),
                //typeof(SolveCollisionSystem),
                typeof(HexMapCollisionSystem),
                typeof(SimpleBehaviourSystem),
                typeof(BeginAttackSystem),
                typeof(ImpactSystem),
                typeof(AutoHealSystem),
                typeof(DamageSystem),
                typeof(MarkDeadSystem),
                typeof(DeadSquadUnitsSystem),
                //typeof(LogHashSumSystem),
            };
            container.RegisterSingleton<ITickableExecutorService, TickableExecutorService>();
            container.Collection.Register<ITickableExecutor>(tickableExecutors, Lifestyle.Singleton);

            container.RegisterSingleton<IPreTickableExecutorService, PreTickableExecutorService>();
            container.RegisterSingleton<IPostTickableExecutorService, PostTickableExecutorService>();

            container.RegisterSingleton<ITargetSearchSystem, TargetSearchSystem>();
            container.RegisterSingleton<ITimelineService, TimelineService>();
        }
    }
}
