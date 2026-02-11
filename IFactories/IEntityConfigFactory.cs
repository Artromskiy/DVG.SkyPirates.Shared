using DVG.Core;
using DVG.Ids;
using DVG.SkyPirates.Shared.Data;
using System;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IEntityConfigFactory<T> : IFactory<ComponentsData, T> where T : struct, IId, IEquatable<T> { }
}
