using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ComponentDependenciesSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly DependencyData[] _dependencies;

        public ComponentDependenciesSystem(IGlobalConfigFactory configFactory, World world)
        {
            var config = configFactory.Create().ComponentDependencies;
            _world = world;

            _dependencies = Array.ConvertAll(config, dependency =>
            {
                HashSet<Type> allComponents = new();
                foreach (var has in dependency.Has)
                    allComponents.UnionWith(has.GetTypes());

                var componentTypes = allComponents.Select(t => Component.GetComponentType(t)).ToArray();
                Signature allSignature = new(componentTypes);
                return new DependencyData(allSignature, dependency.Add, dependency.DefaultOnAdd);
            });
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var data in _dependencies)
            {
                var ensureAction = new AddComponentAction(_world, data.HasComponentSignature, data.DefaultComponentData);
                foreach (var add in data.AddComponentData)
                {
                    add.ForEach(ref ensureAction);
                }
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
                var desc = new QueryDescription(all: _allSignature, none: Component<T>.Signature).NotDisposing();
                var defaultValue = _defaults?.Get<T>();
                _world.Add(desc, defaultValue ?? default);
            }
        }

        private readonly struct DependencyData
        {
            public readonly Signature HasComponentSignature;
            public readonly ComponentsData[] AddComponentData;
            public readonly ComponentsData DefaultComponentData;

            public DependencyData(Signature hasComponentSignature, ComponentsData[] addComponentData, ComponentsData defaultComponentData)
            {
                HasComponentSignature = hasComponentSignature;
                AddComponentData = addComponentData;
                DefaultComponentData = defaultComponentData;
            }
        }
    }
}
