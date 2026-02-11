using DVG.Core.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct GoodsDrop
    {
        public GoodsId Id;
        public int Amount;
        public int MaxCount;
    }
}
