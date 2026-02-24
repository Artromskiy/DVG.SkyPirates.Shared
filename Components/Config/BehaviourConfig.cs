using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Frozen;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct BehaviourConfig
    {
        public FrozenDictionary<StateId, StateId> Scenario;
        public FrozenDictionary<StateId, fix> Durations;
    }
}
