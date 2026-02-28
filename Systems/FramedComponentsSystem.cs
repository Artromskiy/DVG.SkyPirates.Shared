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
    public class FramedComponentsSystem : ITickableExecutor
    {
        private class Description<T>
        {
            public QueryDescription RemoveDesc = new QueryDescription().WithAll<T>();
            public QueryDescription? AddDesc;
        }
        private readonly GenericCreator _creator = new();

        private readonly World _world;
        private readonly DependencyData[] _dependencies;

        public FramedComponentsSystem(FramedComponentDependenciesConfig componentDependencies, World world)
        {
            _world = world;
            _dependencies = componentDependencies.ConvertAll(dependency =>
            {
                HashSet<Type> allComponents = new();
                Signature allSignature = new(Array.ConvertAll(dependency.Has.GetTypes(), Component.GetComponentType));
                return new DependencyData(allSignature, dependency.Add, new());
            }).ToArray();
        }

        public void Tick(int tick, fix deltaTime)
        {
            var removeFramedAction = new ClearFramedAction(_world, _creator);
            FramedComponentsRegistry.ForEachData(ref removeFramedAction);
            foreach (var data in _dependencies)
            {
                var ensureAction = new AddComponentAction(_world, data.HasComponentSignature, data.SignatureCache);
                data.AddComponentData.ForEach(ref ensureAction);
            }
            _world.TrimExcess();
        }

        private readonly struct ClearFramedAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly GenericCreator _creator;

            public ClearFramedAction(World world, GenericCreator creator)
            {
                _world = world;
                _creator = creator;
            }

            public void Invoke<T>() where T : struct
            {
                var desc = _creator.Get<Description<T>>().RemoveDesc;
                _world.Set<T>(desc, default);
            }
        }


        private readonly struct AddComponentAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly Signature _allSignature;
            private readonly GenericCreator _signatureCache;

            public AddComponentAction(World world, Signature allSignature, GenericCreator signatureCache)
            {
                _world = world;
                _allSignature = allSignature;
                _signatureCache = signatureCache;
            }

            public void Invoke<T>() where T : struct
            {
                var descContainer = _signatureCache.Get<Description<T>>();
                descContainer.AddDesc ??= new QueryDescription(all: _allSignature, none: Component<T, Disposing>.Signature);

                var desc = descContainer.AddDesc.Value;
                if (_world.CountEntities(in desc) > 0)
                    _world.Add<T>(in desc);
            }
        }

        private readonly struct DependencyData
        {
            public readonly Signature HasComponentSignature;
            public readonly ComponentsMask AddComponentData;
            public readonly GenericCreator SignatureCache;

            public DependencyData(Signature hasComponentSignature, ComponentsMask addComponentData, GenericCreator signatureCache)
            {
                HasComponentSignature = hasComponentSignature;
                AddComponentData = addComponentData;
                SignatureCache = signatureCache;
            }
        }
    }
}
