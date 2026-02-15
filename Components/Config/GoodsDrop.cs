using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Collections.Immutable;
namespace System.Runtime.CompilerServices
{
    public interface IsExternalInit { }
}
namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public readonly struct GoodsDrop : IDisposable
    {
        public readonly ImmutableSortedDictionary<GoodsId, int> Values { get; init; }

        public readonly void Dispose() { }
    }

    public struct GoodsCollection<T>
    {
        public T Wood;
        public T Spice;
        public T Sugar;
        public T Tea;
        public T Coffee;
        public T Sail;
        public T Rope;
        public T Rum;
    }
}