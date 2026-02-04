using DVG.Core.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct BehaviourState
    {
        public StateId State;
        public fix Percent;
        public fix Duration;

        public StateId? ForceState;

        public override readonly int GetHashCode()
        {
            return Percent.raw + Duration.raw + State.GetHashCode() + (ForceState.HasValue ? ForceState.Value.GetHashCode() : 0);
        }
    }
}
