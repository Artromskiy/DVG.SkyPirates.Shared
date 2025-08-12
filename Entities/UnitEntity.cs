#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;
using DVG.SkyPirates.Shared.Configs;

namespace DVG.SkyPirates.Shared.Entities
{
    public class UnitEntity :
        IMementoable<UnitMemento>,
        ITickable
    {
        public real3 TargetPosition { get; set; }
        public real TargetRotation { get; set; }

        public real3 Position { get; set; }
        public real Rotation { get; set; }
        public real PreAttack { get; set; }
        public real PostAttack { get; set; }

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

        public void Tick(real deltaTime)
        {
            Move(deltaTime);
        }

        private void Move(real deltaTime)
        {
            var direction = TargetPosition.xz - Position.xz;
            Position = real3.MoveTowards(Position, TargetPosition, Config.speed * deltaTime);
            if (real2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                Rotation = Maths.RotateTowards(Rotation, rotation, 720 * deltaTime);
            }
        }
    }
}
