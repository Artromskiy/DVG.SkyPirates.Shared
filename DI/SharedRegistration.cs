using Arch.Core;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Services.CommandExecutors;
using DVG.SkyPirates.Shared.Systems;
using DVG.SkyPirates.Shared.Systems.Special;
using SimpleInjector;
using System;

namespace DVG.SkyPirates.Shared.DI
{
    public static class SharedRegistration
    {
        public static void Register(Container container)
        {
            container.RegisterSingleton(() => World.Create());

            var commandExecutors = new Type[]
            {
                typeof(SpawnSquadCommandExecutor),
                typeof(SpawnUnitCommandExecutor),
                typeof(JoysticCommandExecutor)
                //typeof(CommandLogger)
            };

            World.SharedJobScheduler = new Schedulers.JobScheduler(new Schedulers.JobScheduler.Config() { MaxExpectedConcurrentJobs = 4, ThreadCount = 4, ThreadPrefixName = "ECS_" });
            container.RegisterSingleton(typeof(IEntityConfigFactory<>), typeof(EntityConfigFactory<>));
            container.RegisterSingleton(typeof(IConfigedEntityFactory<>), typeof(ConfigedEntityFactory<>));
            container.RegisterSingleton<IGlobalConfigFactory, GlobalConfigFactory>();
            container.RegisterSingleton<IPackedCirclesFactory, PackedCirclesFactory>();
            container.RegisterSingleton<IWorldDataFactory, WorldDataFactory>();
            container.RegisterSingleton<IEntityFactory, EntityFactory>();
            container.RegisterSingleton<IHexMapFactory, HexMapFactory>();
            container.RegisterSingleton<ISquadFactory, SquadFactory>();

            container.RegisterSingleton<IEntityDependencyService, EntityDependencyService>();
            container.RegisterSingleton<IEntityRegistryService, EntityRegistryService>();

            container.RegisterSingleton<ICommandExecutorService, CommandExecutorService>();
            container.Collection.Register<ICommandExecutor>(commandExecutors, Lifestyle.Singleton);

            container.RegisterSingleton<IPooledItemsProvider, PooledItemsProvider>();
            container.RegisterSingleton<IRollbackHistorySystem, RollbackHistorySystem>();
            container.RegisterSingleton<ISaveHistorySystem, SaveHistorySystem>();
            container.RegisterSingleton<IDisposeSystem, DisposeSystem>();

            var tickableExecutors = new Type[]
            {
                typeof(ComponentDependenciesSystem), // creates dependant components
                typeof(EnsureSystem), // ensures
                typeof(ClearSystem), // cleanups
                typeof(CachePositionSystem),
                typeof(TargetSearchSystem), // cache target search
                typeof(SearchPositionSyncSystem), // sync Positon and TargetSearchPosition
                typeof(SquadMemberCounterSystem), // set count of members to squad
                typeof(SquadTargetSearchDistanceSystem), // set TargetSearchDistance of Squad
                typeof(SquadMemberDestinationSystem), // positioning of squad members
                typeof(SquadMemberSearchSyncSystem), // copies TargetSearch from squad to members
                typeof(DirectionMoveSystem), // moves entities with Direction (squads)
                typeof(FlyMoveSystem),
                typeof(SetSingleTargetSystem),
                typeof(SetMultiTargetSystem),

                typeof(SetTargetDestinationSystem), // sets destination to target or skips
                typeof(MoveSystem),
                typeof(SeparationSystem),
                typeof(HexMapCollisionSystem),
                typeof(SimpleBehaviourSystem),
                typeof(SinglePreAttackSystem),
                typeof(MultiPreAttackSystem),
                typeof(SingleImpactSystem),
                typeof(MultiImpactSystem),
                typeof(AutoHealSystem),
                typeof(DamageSystem),
                typeof(GoodsDropSystem),
                typeof(GoodsCollectorSystem),
                typeof(SquadGoodsDistributionSystem), // distributes goods across squad members
                typeof(MarkDeadSystem),
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
