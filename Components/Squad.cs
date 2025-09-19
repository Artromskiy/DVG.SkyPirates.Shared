using Arch.Core;
using DVG.Core.History.Attributes;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Squad
    {
        public List<Entity> units;

        public override readonly int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < units.Count; i++)
            {
                hash += units[i].Id;
            }
            return hash;
        }
    }
}
