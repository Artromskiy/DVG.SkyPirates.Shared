using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.Ids;
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
                world.Create<Free>(100000);
                return world;
            });

            var commandExecutors = new Type[]
            {
                typeof(SpawnSquadCommandExecutor),
                typeof(SpawnUnitCommandExecutor),
                typeof(JoysticCommandExecutor)
                //typeof(CommandLogger)
            };
            //RegisterSingleton(typeof(IPathFactory<>), typeof(ResourcesFactory<>));

            container.RegisterSingleton(typeof(IEntityConfigFactory<>), typeof(EntityConfigFactory<>));
            container.RegisterSingleton<ISquadFactory, SquadFactory>();
            container.RegisterSingleton<IUnitFactory, UnitFactory>();
            container.RegisterSingleton<ITreeFactory, TreeFactory>();
            container.RegisterSingleton<IFactory<Entity, (TreeId TreeId, int EntityId)>, TreeFactory>();
            container.RegisterSingleton<IRockFactory, RockFactory>();
            container.RegisterSingleton<IFactory<Entity, (RockId RockId, int EntityId)>, RockFactory>();
            container.RegisterSingleton<ICactusFactory, CactusFactory>();
            container.RegisterSingleton<IFactory<Entity, (CactusId CactusId, int EntityId)>, CactusFactory>();

            container.RegisterSingleton<ICommandExecutorService, CommandExecutorService>();
            container.Collection.Register<ICommandExecutor>(commandExecutors, Lifestyle.Singleton);

            var tickableExecutors = new Type[]
            {
                typeof(EnsureSystem),
                typeof(ClearSystem),
                typeof(CachePositionSystem),
                typeof(TargetSearchSystem),
                typeof(TargetSearchSyncSystem),
                typeof(SquadRangeSystem),
                typeof(SquadMoveSystem),
                typeof(SquadUnitsSystem),
                typeof(SetSingleTargetSystem),
                typeof(SetMultiTargetSystem),
                typeof(SetTargetDestinationSystem),
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
