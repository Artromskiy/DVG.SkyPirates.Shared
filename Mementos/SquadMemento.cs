using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Mementos
{
    [Memento]
    public partial struct SquadMemento
    {
        public readonly fix3 Position;
        public readonly fix2 Direction;
        public readonly bool Fixation;
        public readonly int[] UnitsIds;
        public readonly int[] Order;

        public SquadMemento(
            fix3 position,
            fix2 direction,
            bool fixation,
            int[] unitsIds,
            int[] order)
        {
            Position = position;
            Direction = direction;
            Fixation = fixation;
            UnitsIds = unitsIds;
            Order = order;
        }
    }
}
