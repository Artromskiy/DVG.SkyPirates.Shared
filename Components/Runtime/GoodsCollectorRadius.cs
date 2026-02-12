using DVG.Components.Attributes;
using DVG.NewType;
using System;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    public struct GoodsCollectorRadius : INewType
    {
        [Obsolete("Use INewType implicits")]
        public fix Value;
    }
}