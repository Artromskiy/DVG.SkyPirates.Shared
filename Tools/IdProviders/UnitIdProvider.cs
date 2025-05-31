using DVG.Core.Tools;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools.IdProviders
{
    public class UnitIdProvider : StringIdProvider<UnitIdProvider, UnitId>
    {
        public override IEnumerable<UnitId> TypedIds => new UnitId[]
        {
            new UnitId("Sailor"),
            new UnitId("Pirate"),
            new UnitId("Buccaneer"),
            new UnitId("Rogue"),
            new UnitId("Skelly"),
            new UnitId("Archy"),
            new UnitId("Demon"),
            new UnitId("Imp"),
            new UnitId("Militia"),
        };
    }
}
