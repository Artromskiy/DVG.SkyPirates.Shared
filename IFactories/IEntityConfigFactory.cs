using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using System;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IEntityConfigFactory<T> : IFactory<EntityData, T> where T : struct, IId, IEquatable<T> { }
}
