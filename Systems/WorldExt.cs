using System;
using System.Runtime.CompilerServices;
using Delta.ECS;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ecs;

namespace DVG.SkyPirates.Shared.Systems
{
    public static class WorldExt
    {
        public static T FirstOrDefault<T>(this World world) where T : struct
        {
            var state = default(FirstOrDefaultState<T>);
            var queries = ComponentQueryCache<T>.Get(world);
            var filter = queries.Filter;
            world.ForEachEntity(in filter, ref state, queries.Component, typeof(FirstOrDefaultAction<>)).Invoke(ref state);
            return state.Value;
        }

        public static Entity FirstOrDefaultEntity<T>(this World world) where T : struct
        {
            var state = default(FirstOrDefaultState<T>);
            var queries = ComponentQueryCache<T>.Get(world);
            var filter = queries.Filter;
            world.ForEachEntity(in filter, ref state, queries.Component, typeof(FirstOrDefaultAction<>)).Invoke(ref state);
            return state.Entity;
        }

        public static void SetEntityData(this World world, Entity entity, ComponentsSet components)
        {
            var action = new ApplyEntityData(entity, world);
            components.ForEach(ref action);
        }

        internal struct FirstOrDefaultState<T> where T : struct
        {
            public T Value;
            public Entity Entity;
            public bool ValueSet;
        }

        internal struct FirstOrDefaultAction<T> : IForEachContextEntity<FirstOrDefaultState<T>> where T : struct
        {
            public void Invoke(ref FirstOrDefaultState<T> state, EntityRef entity, in T component)
            {
                if (state.ValueSet)
                {
                    return;
                }

                state.ValueSet = true;
                state.Value = component;
                state.Entity = entity.Handle;
            }
        }

        private static class ComponentQueryCache<T> where T : struct
        {
            private static readonly ConditionalWeakTable<World, Lazy<QuerySet>> _queries = new();

            public static QuerySet Get(World world)
                => _queries.GetValue(world, static owner => new Lazy<QuerySet>(() =>
                {
                    var component = owner.Layouts.GetPrimary<T>();
                    return new QuerySet(component, owner.WhereAll(component));
                })).Value;

            internal sealed class QuerySet
            {
                public QuerySet(ComponentId component, Query filter)
                {
                    Component = component;
                    Filter = filter;
                }

                public ComponentId Component { get; }
                public Query Filter { get; }
            }
        }

        internal readonly struct ApplyEntityData : IStructGenericActionArg
        {
            private readonly Entity _entity;
            private readonly World _world;

            public ApplyEntityData(Entity entity, World world)
            {
                _entity = entity;
                _world = world;
            }

            public void Invoke<T>(T component) where T : struct
            {
                _world.GetOrAdd<T>(_entity) = component;
            }
        }
    }
}
