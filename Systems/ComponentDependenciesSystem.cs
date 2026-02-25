using Arch.Core;
using Arch.Core.Utils;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ComponentDependenciesSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly DependencyData[] _dependencies;

        private class Description<T>
        {
            public QueryDescription Desc;
        }

        public ComponentDependenciesSystem(ComponentDependenciesConfig componentDependencies, World world)
        {
            _world = world;
            _dependencies = componentDependencies.ConvertAll(dependency =>
            {
                HashSet<Type> allComponents = new();
                Signature allSignature = new(Array.ConvertAll(dependency.Has.GetTypes(), Component.GetComponentType));
                return new DependencyData(allSignature, dependency.Add, dependency.DefaultOnAdd, new());
            }).ToArray();
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var data in _dependencies)
            {
                var ensureAction = new AddComponentAction(_world, data.HasComponentSignature, data.SignatureCache, data.DefaultComponentData);
                data.AddComponentData.ForEach(ref ensureAction);
            }
            _world.TrimExcess();
        }

        private readonly struct AddComponentAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly Signature _allSignature;
            private readonly GenericCreator _signatureCache;
            private readonly ComponentsSet _defaults;

            public AddComponentAction(World world, Signature allSignature, GenericCreator signatureCache, ComponentsSet defaults)
            {
                _world = world;
                _allSignature = allSignature;
                _signatureCache = signatureCache;
                _defaults = defaults;
            }

            public void Invoke<T>() where T : struct
            {
                var descContainer = _signatureCache.Get<Description<T>>();
                if (descContainer.Desc == default)
                    descContainer.Desc = new QueryDescription(all: _allSignature, none: Component<T, Disposing>.Signature);

                var desc = descContainer.Desc;
                var defaultValue = _defaults?.Get<T>();
                _world.Add(desc, defaultValue ?? default);
            }
        }

        private readonly struct DependencyData
        {
            public readonly Signature HasComponentSignature;
            public readonly ComponentsMask AddComponentData;
            public readonly ComponentsSet DefaultComponentData;
            public readonly GenericCreator SignatureCache;

            public DependencyData(Signature hasComponentSignature, ComponentsMask addComponentData, ComponentsSet defaultComponentData, GenericCreator signatureCache)
            {
                HasComponentSignature = hasComponentSignature;
                AddComponentData = addComponentData;
                DefaultComponentData = defaultComponentData;
                SignatureCache = signatureCache;
            }
        }
    }
}
