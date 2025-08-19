using Arch.Core;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components
{
    public struct Squad
    {
        public List<Entity> units;
        public List<int> orders;
        public fix2[] positions;
    }
}
