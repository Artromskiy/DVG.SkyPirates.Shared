using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class ComponentDependenciesConfig : List<ComponentDependenciesData> { }

    public class ComponentDependenciesData
    {
        public ComponentsMask Has;
        public ComponentsMask Add;
        public ComponentsSet DefaultOnAdd;
    }
}
