using Arch.Core;
using DVG.Core.History.Attributes;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Squad
    {
        public List<Entity> units;
        public List<int> orders;
        public fix2[] positions;
    }
}
