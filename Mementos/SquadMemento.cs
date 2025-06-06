using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Mementos
{
    [Memento]
    public partial struct SquadMemento
    {
        public float3 Position;
        public float Rotation;
        public bool Fixation;
        public int[] UnitsIds;
        public int[] Order;
    }
}
