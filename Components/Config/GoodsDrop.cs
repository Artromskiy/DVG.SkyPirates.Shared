using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Immutable;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public readonly struct GoodsDrop
    {
        public readonly ImmutableSortedDictionary<GoodsId, int> Values { get; init; }
    }
}