using Arch.Core;
using DVG.Core;
using System;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IEntityFactory : IFactory<Entity, int>
    {
        Entity Get(int entityId);
        int Next();
    }

    public interface IEntityFactory<ID> : IFactory<Entity, (ID id, int entityId)>
        where ID : struct, IId, IEquatable<ID>
    { }
}
