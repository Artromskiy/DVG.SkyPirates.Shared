using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Services.CommandExecutors;
using DVG.SkyPirates.Shared.Systems;
using DVG.SkyPirates.Shared.Systems.Special;
using Schedulers;
using SimpleInjector;
using System;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.DI
{
    public class SharedContainer : Container
    {
        public SharedContainer()
        {
            Debug.WriteLine("[DI] SharedContainer Start");
            RegisterSingleton(() =>
            {
                var world = World.Create();
                World.SharedJobScheduler = new JobScheduler(new JobScheduler.Config());
                return world;
            });

            RegisterSingleton(typeof(IEntityConfigFactory<>), typeof(EntityConfigFactory<>));
            RegisterSingleton(typeof(IConfigedEntityFactory<>), typeof(ConfigedEntityFactory<>));
            RegisterFactorySingleton<IGlobalConfigFactory, GlobalConfigFactory, GlobalConfig>();
            RegisterFactorySingleton<IPackedCirclesFactory, PackedCirclesFactory, PackedCirclesConfig>();
            RegisterSingleton<IWorldDataFactory, WorldDataFactory>();
            RegisterSingleton<IEntityFactory, EntityFactory>();
            RegisterSingleton<IHexMapFactory, HexMapFactory>();
            RegisterSingleton<ISquadFactory, SquadFactory>();

            RegisterSingleton<IComponentDependenciesService, ComponentDependenciesService>();
            RegisterSingleton<IComponentDefaultsService, ComponentDefaultsService>();
            RegisterSingleton<IEntityRegistry, EntityRegistry>();

            RegisterSingleton<IPooledItemsProvider, PooledItemsProvider>();
            RegisterSingleton<ITargetSearchSystem, TargetSearchSystem>();

            RegisterSingleton<ITimelineService, TimelineService>();
            RegisterSingleton<ICommandExecutorService, CommandExecutorService>();
            RegisterSingleton<IHistorySystem, HistorySystem>();
            RegisterSingleton<IDisposeSystem, DisposeSystem>();

            RegisterSingleton(typeof(IDeltaTickableService<>), typeof(DeltaTickableService<>));
            RegisterSingleton(typeof(ITickableService<>), typeof(TickableService<>));

            Collection.Register<IDeltaTickableExecutor>(TickableExecutors, Lifestyle.Singleton);
            Collection.Register<ICommandExecutor>(CommandExecutors, Lifestyle.Singleton);

            var globalConfigType = typeof(GlobalConfig);
            foreach (var item in globalConfigType.GetFields())
                RegisterSingleton(item.FieldType, () => item.GetValue(GetInstance<GlobalConfig>()));
        }

        private static Type[] TickableExecutors => new Type[]
        {
            typeof(FramedComponentsSystem), // cleanups
            typeof(FlagDisabledSystem),
            typeof(CachePositionSystem),
            typeof(SetDestinationSystem),
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
            typeof(LoadWorldCommandExecutor),
            typeof(SpawnSquadCommandExecutor),
            typeof(SpawnUnitCommandExecutor),
            typeof(JoysticCommandExecutor)
            //typeof(CommandLogger)
        };

        protected void RegisterFactorySingleton<TService, TImplementation, TInstance>()
            where TImplementation : class, TService
            where TService : class, IFactory<TInstance>
        {
            RegisterSingleton<TService, TImplementation>();
            RegisterSingleton(typeof(TInstance), () => GetInstance<TService>().Create());
        }
    }
}
