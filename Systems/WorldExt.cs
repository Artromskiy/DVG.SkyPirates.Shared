using Delta.ECS;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Systems
{
    public static class WorldExt
    {
        public static void SetEntityData(this World world, Entity entity, ComponentDataSet components)
        {
            foreach (var entry in components)
            {
                entry.Visit(world, new ApplyEntityData(world, entity));
            }
        }

        internal readonly struct ApplyEntityData : IComponentDataVisitor
        {
            private readonly Entity _entity;
            private readonly World _world;

            public ApplyEntityData(World world, Entity entity)
            {
                _entity = entity;
                _world = world;
            }

            public void Visit<T>(string name, in T value) where T : struct
            {
                _world.GetOrAdd<T>(_entity) = value;
            }
        }
    }
}
