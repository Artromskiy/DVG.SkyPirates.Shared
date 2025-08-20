using DVG.Core.Tools;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools.IdProviders
{
    public class GoosdIdProvider : StringIdProvider<GoosdIdProvider, GoodsId>
    {
        public override IEnumerable<GoodsId> TypedIds => new GoodsId[]
        {

        };
    }
}