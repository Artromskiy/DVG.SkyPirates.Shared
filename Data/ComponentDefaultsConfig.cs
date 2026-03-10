using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class ComponentDefaultsConfig : List<ComponentDefaultsData> { }

    public class ComponentDefaultsData
    {
        public ComponentsSet Default = default!;
        public ComponentsMask CopyFrom = default!;
        public ComponentsMask CopyTo = default!;
    }
}
