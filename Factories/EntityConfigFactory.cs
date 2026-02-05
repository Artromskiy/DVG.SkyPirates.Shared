using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using System;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityConfigFactory<T> : IEntityConfigFactory<T> where T : struct, IId, IEquatable<T>
    {
        private readonly IPathFactory<EntityData[]> _pathFactory;
        private readonly EntityData[] _entityDatas;

        public EntityConfigFactory(IPathFactory<EntityData[]> pathFactory)
        {
            _pathFactory = pathFactory;
            _entityDatas = _pathFactory.Create($"Configs/Entities");
        }

        public EntityData Create(T parameters)
        {
            return Array.Find(_entityDatas, d =>
            {
                var t = d.Get<T>();
                return t.HasValue && t.Value.Equals(parameters);
            });
        }
    }
}
