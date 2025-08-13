using DVG.Core;
using DVG.SkyPirates.Shared.Configs;

namespace DVG.SkyPirates.Shared.Entities
{
    public class UnitEntity :
        IMementoable<UnitMemento>,
        IFixedTickable
    {
        public fix3 TargetPosition { get; set; }
        public fix TargetRotation { get; set; }

        public fix3 Position { get; set; }
        public fix Rotation { get; set; }
        public fix PreAttack { get; set; }
        public fix PostAttack { get; set; }

        private UnitConfig Config;

        public UnitMemento GetMemento()
        {
            return new UnitMemento(Position, Rotation, PreAttack, PostAttack);
        }

        public void SetMemento(UnitMemento value)
        {
            Position = value.Position;
            Rotation = value.Rotation;
            PreAttack = value.PreAttack;
            PostAttack = value.PostAttack;
        }

        public UnitEntity(UnitConfig cfg)
        {
            Config = cfg;
        }

        public void Tick(fix deltaTime)
        {
            Move(deltaTime);
        }

        private void Move(fix deltaTime)
        {
            var direction = TargetPosition.xz - Position.xz;
            Position = fix3.MoveTowards(Position, TargetPosition, Config.speed * deltaTime);
            if (fix2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                Rotation = Maths.RotateTowards(Rotation, rotation, 720 * deltaTime);
            }
        }
    }
}
