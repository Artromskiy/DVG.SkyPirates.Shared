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
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.DI
{
    public class SharedContainer : Container
    {
        public SharedContainer()
        {
            Debug.WriteLine("[DI] SharedRegistration start");
            RegisterSingleton(() => World.Create());

            RegisterSingleton(typeof(IEntityConfigFactory<>), typeof(EntityConfigFactory<>));
            RegisterSingleton(typeof(IConfigedEntityFactory<>), typeof(ConfigedEntityFactory<>));
            RegisterSingleton<IGlobalConfigFactory, GlobalConfigFactory>();
            RegisterSingleton<IPackedCirclesFactory, PackedCirclesFactory>();
            RegisterSingleton<IWorldDataFactory, WorldDataFactory>();
            RegisterSingleton<IEntityFactory, EntityFactory>();
            RegisterSingleton<IHexMapFactory, HexMapFactory>();
            RegisterSingleton<ISquadFactory, SquadFactory>();

            RegisterSingleton<IEntityDependencyService, EntityDependencyService>();
            RegisterSingleton<IEntityRegistryService, EntityRegistryService>();

            RegisterSingleton<IPooledItemsProvider, PooledItemsProvider>();
            RegisterSingleton<ITargetSearchSystem, TargetSearchSystem>();

            RegisterSingleton<ITimelineService, TimelineService>();
            RegisterSingleton<ICommandExecutorService, CommandExecutorService>();
            RegisterSingleton<IRollbackHistorySystem, RollbackHistorySystem>();
            RegisterSingleton<ISaveHistorySystem, SaveHistorySystem>();
            RegisterSingleton<IDisposeSystem, DisposeSystem>();

            RegisterSingleton<ITickableExecutorService, TickableExecutorService>();
            RegisterSingleton<IPreTickableExecutorService, PreTickableExecutorService>();
            RegisterSingleton<IPostTickableExecutorService, PostTickableExecutorService>();

            Collection.Register<ITickableExecutor>(TickableExecutors, Lifestyle.Singleton);
            Collection.Register<ICommandExecutor>(CommandExecutors, Lifestyle.Singleton);
        }

        private static Type[] TickableExecutors => new Type[]
        {
            typeof(ComponentDependenciesSystem), // creates dependant components
            typeof(EnsureSystem), // ensures
            typeof(ClearSystem), // cleanups
            typeof(CachePositionSystem),
            typeof(FlagDisabledSystem),
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
            typeof(SimpleHeightSystem),
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

        private static Type[] CommandExecutors => new Type[]
        {
            typeof(SpawnSquadCommandExecutor),
            typeof(SpawnUnitCommandExecutor),
            typeof(JoysticCommandExecutor)
            //typeof(CommandLogger)
        };
    }
}
