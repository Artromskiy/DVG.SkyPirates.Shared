using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.IServices.TargetSearch;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

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

        public void Tick(int tick, fix deltaTime)
        {
            var query = new UnitQuery(deltaTime, _targetSearch);
            _world.InlineQuery<UnitQuery, Unit, Position, Rotation, Fixation, Team>
                (UnitArch.GetQuery(), ref query);
        }

        private readonly struct UnitQuery : IForEach<Unit, Position, Rotation, Fixation, Team>
        {
            private readonly fix _deltaTime;
            private readonly ITargetSearchService _targetSearch;

            public UnitQuery(fix deltaTime, ITargetSearchService targetSearch)
            {
                _deltaTime = deltaTime;
                _targetSearch = targetSearch;
            }

            public void Update(ref Unit u, ref Position p, ref Rotation r, ref Fixation f, ref Team t)
            {
                fix deltaTime = _deltaTime;
                var target = _targetSearch.FindTarget(p.position, 5, t.id);

                if (f.fixation || !target.IsAlive())
                {
                    AbortAttack(ref u, deltaTime);

                    MoveTo(ref p.position, ref r.rotation, u.TargetPosition, u.UnitConfig, deltaTime);
                    return;
                }

                deltaTime = MoveToTarget(target, ref p, ref u, ref r, deltaTime);

                if (deltaTime == 0)
                    return;

                Attack(ref u, target, deltaTime);
            }

            private static void Attack(ref Unit unit, Entity target, fix deltaTime)
            {
                if (unit.PreAttack != 1)
                {
                    var preAttackBefore = unit.PreAttack;
                    DoPreAttack(ref unit.PreAttack, unit.UnitConfig, deltaTime);

                    if (unit.PreAttack == 1 && preAttackBefore != 1 && target.IsAlive())
                        target.Get<Health>().health -= unit.UnitConfig.damage;

                    return;
                }

                if (unit.PreAttack == 1)
                    DoPostAttack(ref unit.PostAttack, unit.UnitConfig, deltaTime);

                if (unit.PostAttack == 1)
                    unit.PreAttack = unit.PostAttack = 0;
            }

            private static void AbortAttack(ref Unit u, fix deltaTime)
            {
                if (u.PreAttack == 1)
                    u.PostAttack = Maths.MoveTowards(u.PostAttack, 1, deltaTime / u.UnitConfig.postAttack);
                else if (u.PreAttack != 0)
                    u.PreAttack = Maths.MoveTowards(u.PostAttack, 0, deltaTime / u.UnitConfig.preAttack);
                else if (u.PostAttack == 1)
                    u.PostAttack = u.PreAttack = 0;
            }

            private static fix MoveToTarget(Entity target, ref Position p, ref Unit u, ref Rotation r, fix deltaTime)
            {
                var targetPosition = target.Get<Position>().position;
                var sqrDistance = fix2.SqrDistance(targetPosition.xz, p.position.xz);
                var sqrAttackDistance = u.UnitConfig.attackDistance * u.UnitConfig.attackDistance;
                if (sqrDistance > sqrAttackDistance)
                {
                    var inAttackRange = fix3.MoveTowards(targetPosition, p.position, u.UnitConfig.attackDistance);
                    deltaTime = MoveTo(ref p.position, ref r.rotation, inAttackRange, u.UnitConfig, deltaTime);
                }
                return deltaTime;
            }

            private static fix MoveTo(ref fix3 position, ref fix rotation, fix3 targetPosition, UnitConfig config, fix deltaTime)
            {
                var direction = targetPosition.xz - position.xz;
                var prevPosition = position;
                position = fix3.MoveTowards(prevPosition, targetPosition, config.speed * deltaTime);
                if (fix2.SqrLength(direction) != 0)
                {
                    var targetRotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                    rotation = Maths.RotateTowards(rotation, targetRotation, 720 * deltaTime);
                }
                return deltaTime - fix2.Distance(position.xz, prevPosition.xz) / config.speed;
            }

            private static void DoPreAttack(ref fix PreAttack, UnitConfig config, fix deltaTime)
            {
                PreAttack = Maths.MoveTowards(PreAttack, 1, deltaTime / config.preAttack);
            }
            private static void DoPostAttack(ref fix PostAttack, UnitConfig config, fix deltaTime)
            {
                PostAttack = Maths.MoveTowards(PostAttack, 1, deltaTime / config.postAttack);
            }
        }

    }
}
