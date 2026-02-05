using DVG.Core.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component]
    public struct BehaviourConfig
    {
        public Dictionary<StateId, StateId> Scenario;
        public Dictionary<StateId, fix> Durations;
    }
}
