using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class ComponentDependenciesConfig : List<ComponentDependenciesData> { }

    public class ComponentDependenciesData
    {
        public ComponentsSet Has;
        public ComponentsSet Add;
        public ComponentsSet DefaultOnAdd;
    }
}
