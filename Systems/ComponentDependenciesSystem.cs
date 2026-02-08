using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ComponentDependenciesSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly DependencyData[] _dependencies;
        private readonly ComponentsData _defaultComponents;

        public ComponentDependenciesSystem(IGlobalConfigFactory configFactory, World world)
        {
            var config = configFactory.Create().ComponentDependencies;
            _world = world;

            _defaultComponents = config.DefaultOnAdd;
            _dependencies = Array.ConvertAll(config.Dependencies, dependency =>
            {
                var componentTypes = Array.ConvertAll(dependency.Has.GetTypes(), t => Component.GetComponentType(t));
                Signature allSignature = new(componentTypes);
                return new DependencyData(allSignature, dependency.Add);
            });
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var data in _dependencies)
            {
                var ensureAction = new AddComponentAction(_world, data.HasComponentSignature, _defaultComponents);
                data.AddComponentData.ForEach(ref ensureAction);
            }
        }

        private readonly struct AddComponentAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly Signature _allSignature;
            private readonly ComponentsData _defaults;

            public AddComponentAction(World world, Signature allSignature, ComponentsData defaults)
            {
                _world = world;
                _allSignature = allSignature;
                _defaults = defaults;
            }

            public void Invoke<T>() where T : struct
            {
                var desc = new QueryDescription(all: _allSignature, none: Component<T>.Signature);
                var defaultValue = _defaults.Get<T>();
                _world.Add(desc, defaultValue ?? default);
            }
        }

        private readonly struct DependencyData
        {
            public readonly Signature HasComponentSignature;
            public readonly ComponentsData AddComponentData;

            public DependencyData(Signature hasComponentSignature, ComponentsData addComponentData)
            {
                HasComponentSignature = hasComponentSignature;
                AddComponentData = addComponentData;
            }
        }
    }
}
