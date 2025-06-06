using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Presenters
{
    [Memento]
    public partial struct UnitMemento
    {
        public float3 Position;
        public float Rotation;
        public float StatePercent;
        public int State;
    }
}
