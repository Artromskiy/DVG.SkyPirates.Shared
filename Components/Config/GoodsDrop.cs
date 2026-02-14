using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct GoodsDrop : IDisposable
    {
        public SortedList<GoodsId, int> Values;

        public readonly void Dispose()
        {
            // return this thing to pool
        }
    }
}