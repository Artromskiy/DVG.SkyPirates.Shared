using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    public struct BehaviourState
    {
        public StateId State;
        public fix Percent;
        public fix Duration;

        public StateId? ForceState;
    }
}
