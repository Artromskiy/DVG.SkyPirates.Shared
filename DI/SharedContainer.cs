using Delta.ECS;
using DVG;
using DVG.Components;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ecs;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Services.CommandExecutors;
using DVG.SkyPirates.Shared.Services.CommandMutators;
using DVG.SkyPirates.Shared.Services.CommandValidators;
using DVG.SkyPirates.Shared.Systems;
using DVG.SkyPirates.Shared.Systems.Special;
using SimpleInjector;
using System;

namespace DVG.SkyPirates.Shared.DI
{
    public class SharedContainer : Container
    {
        public SharedContainer()
        {
            Delta.Diagnostics.Trace.Info("[DI] SharedContainer Start");
            RegisterSingleton(CreateWorld);

            RegisterSingleton<TimelineWriter>();

            RegisterSingleton(typeof(IEntityConfigFactory<>), typeof(EntityConfigFactory<>));
            RegisterSingleton(typeof(IConfigedEntityFactory<>), typeof(ConfigedEntityFactory<>));
            RegisterFactorySingleton<IGlobalConfigFactory, GlobalConfigFactory, GlobalConfig>();
            RegisterFactorySingleton<IPackedCirclesFactory, PackedCirclesFactory, PackedCirclesConfig>();
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
            RegisterSingleton<ICommandAcceptanceService, CommandAcceptanceService>();
            RegisterSingleton<IHistorySystem, HistorySystem>();
            RegisterSingleton<IDisposeSystem, DisposeSystem>();

            RegisterSingleton(typeof(IDeltaTickableService<>), typeof(DeltaTickableService<>));
            RegisterSingleton(typeof(ITickableService<>), typeof(TickableService<>));

            Collection.Register<IDeltaTickableExecutor>(TickableExecutors, Lifestyle.Singleton);
            Collection.Register<ICommandExecutorRegistration>(CommandExecutors, Lifestyle.Singleton);
            Collection.Register<ICommandValidatorRegistration>(CommandValidators, Lifestyle.Singleton);
            Collection.Register<ICommandMutatorRegistration>(CommandMutators, Lifestyle.Singleton);

            var globalConfigType = typeof(GlobalConfig);
            foreach (var item in globalConfigType.GetFields())
            {
                RegisterSingleton(item.FieldType, () => item.GetValue(GetInstance<GlobalConfig>()));
            }
        }

        private static Type[] TickableExecutors => new Type[]
        {
            typeof(FramedComponentsSystem), // cleanups
            typeof(FlagDisabledSystem),
            typeof(SquadUnitMergeSystem), // merge groups of three units
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
            typeof(JoystickCommandExecutor)
            //typeof(CommandLogger)
        };

        private static Type[] CommandValidators => new Type[]
        {
            typeof(FutureCommandValidator<InvalidateCommand>), typeof(LateCommandValidator<InvalidateCommand>), typeof(ZeroTickCommandValidator<InvalidateCommand>),
            typeof(FutureCommandValidator<JoystickCommand>), typeof(LateCommandValidator<JoystickCommand>), typeof(ZeroTickCommandValidator<JoystickCommand>),
            typeof(FutureCommandValidator<LoadWorldCommand>), typeof(LateCommandValidator<LoadWorldCommand>), typeof(ZeroTickCommandValidator<LoadWorldCommand>),
            typeof(FutureCommandValidator<SetEntityDataCommand>), typeof(LateCommandValidator<SetEntityDataCommand>), typeof(ZeroTickCommandValidator<SetEntityDataCommand>),
            typeof(FutureCommandValidator<SpawnSquadCommand>), typeof(LateCommandValidator<SpawnSquadCommand>), typeof(ZeroTickCommandValidator<SpawnSquadCommand>),
            typeof(FutureCommandValidator<SpawnUnitCommand>), typeof(LateCommandValidator<SpawnUnitCommand>), typeof(ZeroTickCommandValidator<SpawnUnitCommand>),
            typeof(FutureCommandValidator<TickSyncCommand>), typeof(LateCommandValidator<TickSyncCommand>), typeof(ZeroTickCommandValidator<TickSyncCommand>),
        };

        private static Type[] CommandMutators => new Type[]
        {
            typeof(SpawnCommandMutator),
        };

        private static World CreateWorld()
        {
            var layouts = new ComponentLayoutRegistry();

            var registerComponents = new RegisterComponentLayouts(layouts);
            ComponentsRegistry.ForEachData(ref registerComponents);

            var registerHistory = new RegisterHistoryLayouts(layouts);
            HistoryComponentsRegistry.ForEachData(ref registerHistory);

            layouts.Register<Disabled>(GetSchemaId(typeof(Disabled).FullName));
            layouts.Register<Temp>(GetSchemaId(typeof(Temp).FullName));

            var world = new World(layouts, 1024);
            _ = WorldComponentIds.For(world);
            return world;
        }

        private static SchemaId GetSchemaId(string schemaName)
        {
            // Keep ids stable across worlds and independent of generated registry order.
            const ulong offsetBasis = 14695981039346656037UL;
            const ulong prime = 1099511628211UL;

            ulong hash = offsetBasis;
            foreach (char character in schemaName)
            {
                hash ^= character;
                hash *= prime;
            }

            return new SchemaId(hash);
        }

        private readonly struct RegisterComponentLayouts : IStructGenericAction
        {
            private readonly ComponentLayoutRegistry _layouts;

            public RegisterComponentLayouts(ComponentLayoutRegistry layouts)
            {
                _layouts = layouts;
            }

            public readonly void Invoke<T>() where T : struct
            {
                _layouts.Register<T>(GetSchemaId(typeof(T).FullName));
            }
        }

        private readonly struct RegisterHistoryLayouts : IStructGenericAction
        {
            private readonly ComponentLayoutRegistry _layouts;

            public RegisterHistoryLayouts(ComponentLayoutRegistry layouts)
            {
                _layouts = layouts;
            }

            public readonly void Invoke<T>() where T : struct
            {
                string schemaName = typeof(History<>).FullName + "<" + typeof(T).FullName + ">";
                var componentId = _layouts.GetPrimary<T>();
                _layouts.Register(typeof(History<>), componentId, GetSchemaId(schemaName));
            }
        }

        protected void RegisterFactorySingleton<TService, TImplementation, TInstance>()
            where TImplementation : class, TService
            where TService : class, IFactory<TInstance>
        {
            RegisterSingleton<TService, TImplementation>();
            RegisterSingleton(typeof(TInstance), () => GetInstance<TService>().Create()!);
        }
    }
}
