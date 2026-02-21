using DVG.Ids;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityConfigFactory<T> : IEntityConfigFactory<T> where T : struct, IId, IEquatable<T>
    {
        private readonly Dictionary<T, ComponentsData> _entities = new();

        public EntityConfigFactory(IGlobalConfigFactory globalConfigFactory)
        {
            var config = globalConfigFactory.Create();
            foreach (var item in config.UnitsStats)
                TryAdd(item);
            foreach (var item in config.Cactuses)
                TryAdd(item);
            foreach (var item in config.Trees)
                TryAdd(item);
            foreach (var item in config.Rocks)
                TryAdd(item);
            foreach (var item in config.Goods)
                TryAdd(item);
        }

        private void TryAdd(ComponentsData data)
        {
            var key = data.Get<T>();
            if (key != null)
                _entities.Add(key.Value, data);
        }

        public ComponentsData Create(T parameters)
        {
            _entities.TryGetValue(parameters, out var entityData);
            return entityData;
        }
    }
}
