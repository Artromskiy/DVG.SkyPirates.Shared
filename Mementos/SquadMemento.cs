#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Mementos
{
    [Memento]
    public partial struct SquadMemento
    {
        public readonly real3 Position;
        public readonly real Rotation;
        public readonly bool Fixation;
        public readonly int[] UnitsIds;
        public readonly int[] Order;

        public SquadMemento(real3 position, real rotation, bool fixation, int[] unitsIds, int[] order)
        {
            Position = position;
            Rotation = rotation;
            Fixation = fixation;
            UnitsIds = unitsIds;
            Order = order;
        }
    }
}
