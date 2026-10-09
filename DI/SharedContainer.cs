using Delta.ECS;
using DVG;
using DVG.Components;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ecs;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.Services;
using DVG.SkyPirates.Shared.Services.CommandExecutors;
using DVG.SkyPirates.Shared.Services.CommandMutators;
using DVG.SkyPirates.Shared.Services.CommandValidators;
using DVG.SkyPirates.Shared.Services.Netcode;
using DVG.SkyPirates.Shared.Systems;
using DVG.SkyPirates.Shared.Systems.Special;
using SimpleInjector;
using System;
using Delta.Netcode;

namespace DVG.SkyPirates.Shared.DI
{
    public class SharedContainer : Container
    {
        public SharedContainer(SessionMode sessionMode = SessionMode.Local, bool registerCommandReceiver = true)
        {
            Delta.Diagnostics.Trace.Info("[DI] SharedContainer Start");
            RegisterSingleton(CreateWorld);
            RegisterSingleton(() => new WorldHistory(
                GetInstance<World>(),
                WorldComponentIds.For(GetInstance<World>()).History,
                4,
                Constants.MaxHistoryTicks));

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

            RegisterSingleton<IHistorySystem, HistorySystem>();
            RegisterSingleton<IDisposeSystem, DisposeSystem>();
            if (registerCommandReceiver)
            {
                RegisterSingleton<ICommandReciever, CommandReceiver>();
            }

            RegisterSingleton<SkyPiratesSessionSetup>();
            RegisterSingleton<SkyPiratesSessionTickLoop>();
            RegisterSingleton<ISessionTransport>(() => CreateSessionTransport(sessionMode));
            RegisterSingleton(() => new SkyPiratesSessionProvider(
                GetInstance<SkyPiratesSessionSetup>(),
                sessionMode,
                GetInstance<ISessionTransport>(),
                GetInstance<ICommandReciever>()));

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
            typeof(SetEntityDataCommandExecutor),
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
            var world = new World(initialEntityCapacity: 1024);
            RegisterAll(world);
            _ = WorldComponentIds.For(world);
            return world;
        }

        private static void RegisterAll(World world)
        {
            var layouts = world.Layouts;
            layouts.Register<ActivityRange>(GetSchemaId(typeof(ActivityRange).FullName));
            layouts.Register<Alive>(GetSchemaId(typeof(Alive).FullName));
            layouts.Register<AutoHeal>(GetSchemaId(typeof(AutoHeal).FullName));
            layouts.Register<BehaviourConfig>(GetSchemaId(typeof(BehaviourConfig).FullName));
            layouts.Register<BehaviourState>(GetSchemaId(typeof(BehaviourState).FullName));
            layouts.Register<CachePosition>(GetSchemaId(typeof(CachePosition).FullName));
            layouts.Register<CactusId>(GetSchemaId(typeof(CactusId).FullName));
            layouts.Register<ClientId>(GetSchemaId(typeof(ClientId).FullName));
            layouts.Register<Collide>(GetSchemaId(typeof(Collide).FullName));
            layouts.Register<Damage>(GetSchemaId(typeof(Damage).FullName));
            layouts.Register<Destination>(GetSchemaId(typeof(Destination).FullName));
            layouts.Register<Direction>(GetSchemaId(typeof(Direction).FullName));
            layouts.Register<Fixation>(GetSchemaId(typeof(Fixation).FullName));
            layouts.Register<FlyDestination>(GetSchemaId(typeof(FlyDestination).FullName));
            layouts.Register<GoodsAmount>(GetSchemaId(typeof(GoodsAmount).FullName));
            layouts.Register<GoodsCollectorRadius>(GetSchemaId(typeof(GoodsCollectorRadius).FullName));
            layouts.Register<GoodsDrop>(GetSchemaId(typeof(GoodsDrop).FullName));
            layouts.Register<GoodsId>(GetSchemaId(typeof(GoodsId).FullName));
            layouts.Register<Health>(GetSchemaId(typeof(Health).FullName));
            layouts.Register<HexMap>(GetSchemaId(typeof(HexMap).FullName));
            layouts.Register<ImpactDistance>(GetSchemaId(typeof(ImpactDistance).FullName));
            layouts.Register<Level>(GetSchemaId(typeof(Level).FullName));
            layouts.Register<MaxHealth>(GetSchemaId(typeof(MaxHealth).FullName));
            layouts.Register<MaxSpeed>(GetSchemaId(typeof(MaxSpeed).FullName));
            layouts.Register<Position>(GetSchemaId(typeof(Position).FullName));
            layouts.Register<Radius>(GetSchemaId(typeof(Radius).FullName));
            layouts.Register<RandomSeed>(GetSchemaId(typeof(RandomSeed).FullName));
            layouts.Register<RecivedDamage>(GetSchemaId(typeof(RecivedDamage).FullName));
            layouts.Register<RockId>(GetSchemaId(typeof(RockId).FullName));
            layouts.Register<Rotation>(GetSchemaId(typeof(Rotation).FullName));
            layouts.Register<Separation>(GetSchemaId(typeof(Separation).FullName));
            layouts.Register<Separator>(GetSchemaId(typeof(Separator).FullName));
            layouts.Register<Squad>(GetSchemaId(typeof(Squad).FullName));
            layouts.Register<SquadMember>(GetSchemaId(typeof(SquadMember).FullName));
            layouts.Register<SquadMemberCount>(GetSchemaId(typeof(SquadMemberCount).FullName));
            layouts.Register<SyncId>(GetSchemaId(typeof(SyncId).FullName));
            layouts.Register<SyncIdReserve>(GetSchemaId(typeof(SyncIdReserve).FullName));
            layouts.Register<Target>(GetSchemaId(typeof(Target).FullName));
            layouts.Register<Targets>(GetSchemaId(typeof(Targets).FullName));
            layouts.Register<TargetSearchDistance>(GetSchemaId(typeof(TargetSearchDistance).FullName));
            layouts.Register<TargetSearchPosition>(GetSchemaId(typeof(TargetSearchPosition).FullName));
            layouts.Register<TeamId>(GetSchemaId(typeof(TeamId).FullName));
            layouts.Register<TreeId>(GetSchemaId(typeof(TreeId).FullName));
            layouts.Register<UnitId>(GetSchemaId(typeof(UnitId).FullName));
            layouts.Register<Disabled>(GetSchemaId(typeof(Disabled).FullName));
            layouts.Register<Temp>(GetSchemaId(typeof(Temp).FullName));
        }

        private ISessionTransport CreateSessionTransport(SessionMode mode)
        {
            switch (mode)
            {
                case SessionMode.Client:
                    return new RiptideClientSessionTransport(GetInstance<Riptide.Client>());
                case SessionMode.Server:
                    return new RiptideServerSessionTransport(GetInstance<Riptide.Server>());
                default:
                    return new NullSessionTransport();
            }
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

        protected void RegisterFactorySingleton<TService, TImplementation, TInstance>()
            where TImplementation : class, TService
            where TService : class, IFactory<TInstance>
        {
            RegisterSingleton<TService, TImplementation>();
            RegisterSingleton(typeof(TInstance), () => GetInstance<TService>().Create()!);
        }
    }
}
