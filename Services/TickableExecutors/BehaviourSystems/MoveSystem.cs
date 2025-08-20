using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    public class MoveSystem : ITickableExecutor
    {
        private readonly World _world;
        public MoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new MoveQuery(deltaTime);
            var desc = new QueryDescription().WithAll<Behaviour, Position, Rotation, TargetPosition, TargetRotation, MoveSpeed>();
            _world.InlineQuery<MoveQuery, Behaviour, Position, Rotation, TargetPosition, TargetRotation, MoveSpeed>
                (desc, ref query);
        }

        private readonly struct MoveQuery :
            IForEach<Behaviour, Position, Rotation, TargetPosition, TargetRotation, MoveSpeed>
        {
            private readonly fix _deltaTime;

            public MoveQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(
                ref Behaviour behaviour,
                ref Position position,
                ref Rotation rotation,
                ref TargetPosition targetPosition,
                ref TargetRotation targetRotation,
                ref MoveSpeed moveSpeed)
            {
                if (behaviour.State != StateId.Constants.Move)
                    return;

                MoveTo(ref position.Value, ref rotation.Value, targetPosition.Value, targetRotation.Value, moveSpeed.Value, _deltaTime);
            }

            private static fix MoveTo(
                ref fix3 position,
                ref fix rotation,
                fix3 targetPosition,
                fix targetRotation,
                fix speed,
                fix deltaTime)
            {
                var direction = targetPosition.xz - position.xz;
                var prevPosition = position;
                position = fix3.MoveTowards(prevPosition, targetPosition, speed * deltaTime);

                fix rotateTo = fix2.SqrLength(direction) != 0 ?
                    Maths.Degrees(-Maths.Atan2(-direction.x, direction.y)) :
                    targetRotation;

                rotation = Maths.RotateTowards(rotation, rotateTo, 720 * deltaTime);
                return deltaTime - fix2.Distance(position.xz, prevPosition.xz) / speed;
            }
        }
    }
}
