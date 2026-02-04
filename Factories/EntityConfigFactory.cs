using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using System;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityConfigFactory : IEntityConfigFactory
    {
        private readonly IPathFactory<EntityData[]> _pathFactory;

        public EntityConfigFactory(IPathFactory<EntityData[]> pathFactory)
        {
            _pathFactory = pathFactory;
        }

        public EntityData Create(IId parameters)
        {
            var entityDatas = _pathFactory.Create($"Configs/Units");
            return Array.Find(entityDatas, d =>
            {
                var t = d.Get<UnitId>();
                return t.HasValue && t.Value.Equals(t.Value);
            });
        }
    }
}
