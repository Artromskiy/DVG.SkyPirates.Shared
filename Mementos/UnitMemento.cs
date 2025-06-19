using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Presenters
{
    [Memento]
    public partial struct UnitMemento
    {
        public readonly float3 Position;
        public readonly float Rotation;
        public readonly float StatePercent;
        public readonly int State;

        public UnitMemento(float3 position, float rotation, float statePercent, int state)
        {
            Position = position;
            Rotation = rotation;
            StatePercent = statePercent;
            State = state;
        }
    }
}
