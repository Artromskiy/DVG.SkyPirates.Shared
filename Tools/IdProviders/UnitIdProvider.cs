using DVG.Core.Tools;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools.IdProviders
{
    public class UnitIdProvider : StringIdProvider<UnitIdProvider, UnitId>
    {
        public override IEnumerable<UnitId> TypedIds => new UnitId[]
        {
            new("Sailor"),
            new("Pirate"),
            new("Buccaneer"),
            new("Rogue"),
            new("Skelly"),
            new("Archy"),
            new("Demon"),
            new("Imp"),
            new("Militia"),
        };
    }
}
