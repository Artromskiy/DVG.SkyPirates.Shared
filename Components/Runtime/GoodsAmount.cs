using DVG.Components.Attributes;
using DVG.NewType;
using System;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct GoodsAmount : INewType
    {
        [Obsolete]
        public int Value;
    }
}
