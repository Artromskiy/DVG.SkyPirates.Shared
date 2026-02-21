using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class ComponentDependenciesConfig : List<ComponentDependenciesData> { }

    public class ComponentDependenciesData
    {
        public ComponentsData Has;
        public ComponentsData Add;
        public ComponentsData DefaultOnAdd;
    }
}
