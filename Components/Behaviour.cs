using DVG.Core.History.Attributes;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Behaviour
    {
        public StateId State;
        public fix Percent;
        public fix Duration;

        public StateId? ForceState;
    }
}
