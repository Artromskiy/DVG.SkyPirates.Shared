using DVG.Core.History.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Behaviour
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
