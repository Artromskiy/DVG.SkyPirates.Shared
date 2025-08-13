using DVG.Core.Mementos.Attributes;

namespace DVG.SkyPirates.Shared.Entities
{
    [Memento]
    public partial struct UnitMemento
    {
        public readonly fix3 Position;
        public readonly fix Rotation;
        public readonly fix PreAttack;
        public readonly fix PostAttack;

        public UnitMemento(fix3 position, fix rotation, fix preAttack, fix postAttack)
        {
            Position = position;
            Rotation = rotation;
            PreAttack = preAttack;
            PostAttack = postAttack;
        }
    }
}
