using Arch.Core;
using DVG.Core;
using System;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IConfigedEntityFactory<T> : IFactory<Entity, (T Id, int SyncId)> where T : struct, IId, IEquatable<T> { }
}
