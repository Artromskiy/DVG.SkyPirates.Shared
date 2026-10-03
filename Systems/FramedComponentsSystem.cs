using Delta;
using Delta.ECS;
using DVG.SkyPirates.Shared.Ecs;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class FramedComponentsSystem : IDeltaTickableExecutor
    {
        private readonly World _world;
        private readonly DependencyData[] _dependencies;
        private readonly ComponentId[] _framedComponentIds;
        private readonly Query[] _clearQueries;

        public FramedComponentsSystem(FramedComponentDependenciesConfig componentDependencies, World world)
        {
            _world = world;
            _framedComponentIds = WorldComponentIds.For(world).Framed;
            _clearQueries = Array.ConvertAll(_framedComponentIds, id => world.WhereAll(id));
            _dependencies = componentDependencies.ConvertAll(dependency => CreateDependency(world, dependency)).ToArray();
        }

        public void Tick(int tick, fix deltaTime)
        {
            for (int i = 0; i < _framedComponentIds.Length; i++)
            {
                var query = _clearQueries[i];
                _world.ForEach(in query, _framedComponentIds[i], typeof(ClearFramedComponent<>));
            }

            Span<ComponentId> component = stackalloc ComponentId[1];
            foreach (var data in _dependencies)
            {
                for (int i = 0; i < data.AddComponentIds.Length; i++)
                {
                    var query = data.AddQueries[i];
                    component[0] = data.AddComponentIds[i];
                    _world.Add(in query, component);
                }
            }
        }

        internal struct ClearFramedComponent<T> : IForEach where T : struct
        {
            public void Invoke(ref T component) => component = default;
        }

        private static DependencyData CreateDependency(World world, ComponentDependenciesData dependency)
        {
            var types = dependency.Has.GetTypes();
            var has = new ComponentId[types.Length + 1];
            for (int i = 0; i < types.Length; i++)
            {
                has[i] = world.Layouts.GetPrimary(types[i]);
            }

            has[types.Length] = world.Layouts.GetPrimary<Alive>();

            var add = new List<ComponentId>();
            var collect = new CollectComponentIds(world, add);
            dependency.Add.ForEach(ref collect);

            var addIds = add.ToArray();
            var queries = new Query[addIds.Length];
            var filter = world.WhereAll(has);
            for (int i = 0; i < addIds.Length; i++)
            {
                queries[i] = filter.WhereNone(addIds[i]);
            }

            return new DependencyData(addIds, queries);
        }

        private readonly struct CollectComponentIds : IStructGenericAction
        {
            private readonly World _world;
            private readonly List<ComponentId> _componentIds;

            public CollectComponentIds(World world, List<ComponentId> componentIds)
            {
                _world = world;
                _componentIds = componentIds;
            }

            public void Invoke<T>() where T : struct
                => _componentIds.Add(_world.Layouts.GetPrimary<T>());
        }

        internal readonly struct DependencyData
        {
            public readonly ComponentId[] AddComponentIds;
            public readonly Query[] AddQueries;

            public DependencyData(ComponentId[] addComponentIds, Query[] addQueries)
            {
                AddComponentIds = addComponentIds;
                AddQueries = addQueries;
            }
        }
    }
}
