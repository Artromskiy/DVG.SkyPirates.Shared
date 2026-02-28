using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class ComponentDefaultsConfig : List<ComponentDefaultsData> { }

    public class ComponentDefaultsData
    {
        public ComponentsSet Default;
        public ComponentsMask CopyFrom;
        public ComponentsMask CopyTo;
    }
}
