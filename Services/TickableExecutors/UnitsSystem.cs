using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.ComponentsQueries;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TargetSearch;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    public class UnitsSystem : ITickableExecutor
    {
        private readonly ITargetSearchService _targetSearch;
        private readonly World _world;

        public UnitsSystem(ITargetSearchService targetSearch, World world)
        {
            _targetSearch = targetSearch;
            _world = world;
        }

        public void Tick(fix deltaTime)
        {
            _world.Query(new UnitQuery(), (ref Fixation f, ref Unit u, ref Position p, ref Rotation r, ref Team t, ref Health h) =>
            {
                Tick(
                    ref u.TargetPosition,
                    ref p.position,
                    ref r.rotation,
                    ref u.PreAttack,
                    ref u.PostAttack,
                    ref f.fixation,
                    ref t.id,
                    u.UnitConfig,
                    deltaTime);
            });
        }


        public void Tick(
            ref fix3 TargetPosition,
            ref fix3 Position,
            ref fix Rotation,
            ref fix PreAttack,
            ref fix PostAttack,
            ref bool Fixation,
            ref int TeamId,
            UnitConfig config,
            fix deltaTime)
        {
            if (Fixation)
            {
                MoveTo(ref Position, ref Rotation, TargetPosition, config, deltaTime);
                return;
            }

            var target = _targetSearch.FindTarget(Position, 5, TeamId);
            if (target == null)
            {
                PreAttack = 0;
                PostAttack = 0;
                MoveTo(ref Position, ref Rotation, TargetPosition, config, deltaTime);
                return;
            }

            var sqrDistance = fix2.SqrDistance(target.Get<Position>().position.xz, Position.xz);
            var sqrAttackDistance = config.attackDistance * config.attackDistance;
            if (sqrDistance < sqrAttackDistance)
            {
                var inAttackRange = fix3.MoveTowards(target.Get<Position>().position, Position, config.attackDistance);
                deltaTime = MoveTo(ref Position, ref Rotation, inAttackRange, config, deltaTime);
            }

            if (deltaTime == 0)
                return;

            if (PreAttack != 1)
            {
                var preAttackBefore = PreAttack;
                DoPreAttack(ref PreAttack, config, deltaTime);

                if (PreAttack == 1 && preAttackBefore != 1)
                    target.Get<Health>().health -= config.damage;

                return;
            }

            if (PreAttack == 1)
                DoPostAttack(ref PostAttack, config, deltaTime);

            if (PreAttack == 1 && PostAttack == 1)
            {
                PreAttack = 0;
                PostAttack = 0;
            }
        }

        private fix MoveTo(ref fix3 position, ref fix rotation, fix3 targetPosition, UnitConfig config, fix deltaTime)
        {
            var direction = targetPosition.xz - position.xz;
            var prevPosition = position;
            position = fix3.MoveTowards(prevPosition, targetPosition, config.speed * deltaTime);
            if (fix2.SqrLength(direction) != 0)
            {
                var targetRotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                rotation = Maths.RotateTowards(rotation, targetRotation, 720 * deltaTime);
            }
            return fix2.Distance(position.xz, prevPosition.xz) / (config.speed * deltaTime);
        }

        private void DoPreAttack(ref fix PreAttack, UnitConfig config, fix deltaTime)
        {
            fix prevPreAttack = PreAttack * config.preAttack;
            fix newPreAttack = Maths.MoveTowards(prevPreAttack, config.preAttack, deltaTime);
            PreAttack = newPreAttack / config.preAttack;
        }
        private void DoPostAttack(ref fix PostAttack, UnitConfig config, fix deltaTime)
        {
            fix prevPostAttack = PostAttack * config.postAttack;
            fix newPostAttack = Maths.MoveTowards(prevPostAttack, config.postAttack, deltaTime);
            PostAttack = newPostAttack / config.postAttack;
        }
    }
}
