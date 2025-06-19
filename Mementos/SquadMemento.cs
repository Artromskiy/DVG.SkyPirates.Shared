using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Mementos
{
    [Memento]
    public partial struct SquadMemento
    {
        public readonly float3 Position;
        public readonly float Rotation;
        public readonly bool Fixation;
        public readonly int[] UnitsIds;
        public readonly int[] Order;

        public SquadMemento(float3 position, float rotation, bool fixation, int[] unitsIds, int[] order)
        {
            Position = position;
            Rotation = rotation;
            Fixation = fixation;
            UnitsIds = unitsIds;
            Order = order;
        }
    }
}
