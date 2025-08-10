#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Presenters
{
    [Memento]
    public partial struct UnitMemento
    {
        public readonly real3 Position;
        public readonly real Rotation;
        public readonly real StatePercent;
        public readonly int State;

        public UnitMemento(real3 position, real rotation, real statePercent, int state)
        {
            Position = position;
            Rotation = rotation;
            StatePercent = statePercent;
            State = state;
        }
    }
}
