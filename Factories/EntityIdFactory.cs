using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems.Special;
using System;

namespace DVG.SkyPirates.Shared.Factories
{
    [Obsolete]
    public class EntityIdFactory<ID> : IEntityFactory<ID> where ID : struct, IId, IEquatable<ID>
    {
        private readonly IEntityFactory _entityFactory;
        private readonly IEntityConfigFactory<ID> _entityConfigFactory;
        private readonly World _world;

        public Entity Create((ID id, int entityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.id);
            var entity = _entityFactory.Create(parameters.entityId);
            _world.SetEntityData(entity, config);
            return entity;
        }
    }
}
