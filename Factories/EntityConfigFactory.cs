using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityConfigFactory<T> : IEntityConfigFactory<T> where T : struct, IId, IEquatable<T>
    {
        private readonly IPathFactory<EntityData[]> _pathFactory;

        private readonly EntityData[] _entityDatas;
        private readonly Dictionary<T, EntityData> _entities;

        public EntityConfigFactory(IPathFactory<EntityData[]> pathFactory)
        {
            _pathFactory = pathFactory;
            _entityDatas = _pathFactory.Create($"Configs/Entities");
            _entities = new();
            foreach (var item in _entityDatas)
            {
                var key = item.Get<T>();
                if (key != null)
                    _entities.Add(key.Value, item);
            }
        }

        public EntityData Create(T parameters)
        {
            _entities.TryGetValue(parameters, out var entityData);
            return entityData;
        }
    }
}
