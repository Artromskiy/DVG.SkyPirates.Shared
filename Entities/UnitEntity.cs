using DVG.Core;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.IServices.TargetSearch;

namespace DVG.SkyPirates.Shared.Entities
{
    public class UnitEntity :
        IMementoable<UnitMemento>,
        IFixedTickable,
        ITarget
    {
        public fix3 TargetPosition { get; set; }
        public fix TargetRotation { get; set; }

        public fix3 Position { get; set; }
        public fix Rotation { get; set; }
        public fix PreAttack { get; set; }
        public fix PostAttack { get; set; }
        public bool Fixation { get; set; }

        public int TeamId { get; private set; }
        public fix Health { get; set; }

        private readonly UnitConfig _config;
        private readonly ITargetSearchService _targetSearch;

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

        public UnitEntity(ITargetSearchService targetSearch, UnitConfig cfg, int teamId)
        {
            _config = cfg;
            _targetSearch = targetSearch;
            Health = (int)cfg.health;
            TeamId = teamId;
        }

        public void Tick(fix deltaTime)
        {
            if (Fixation)
            {
                PreAttack = 0;
                PostAttack = 0;
                MoveTo(TargetPosition, deltaTime);
                return;
            }

            var target = _targetSearch.FindTarget(Position, 5, TeamId);
            if (target == null)
            {
                PreAttack = 0;
                PostAttack = 0;
                MoveTo(TargetPosition, deltaTime);
                return;
            }

            var sqrDistance = fix2.SqrDistance(target.Position.xz, Position.xz);
            var sqrAttackDistance = _config.attackDistance * _config.attackDistance;
            if (sqrDistance < sqrAttackDistance)
            {
                var inAttackRange = fix3.MoveTowards(target.Position, Position, _config.attackDistance);
                deltaTime = MoveTo(inAttackRange, deltaTime);
            }

            if (deltaTime == 0)
                return;

            if (PreAttack != 1)
            {
                var preAttackBefore = PreAttack;
                DoPreAttack(deltaTime);

                if (PreAttack == 1 && preAttackBefore != 1)
                    target.Health -= _config.damage;

                return;
            }

            if(PreAttack == 1)
                DoPostAttack(deltaTime);

            if(PreAttack == 1 && PostAttack == 1)
            {
                PreAttack = 0;
                PostAttack = 0;
            }
        }

        private fix MoveTo(fix3 targetPosition, fix deltaTime)
        {
            var direction = targetPosition.xz - Position.xz;
            var prevPosition = Position;
            Position = fix3.MoveTowards(prevPosition, targetPosition, _config.speed * deltaTime);
            if (fix2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                Rotation = Maths.RotateTowards(Rotation, rotation, 720 * deltaTime);
            }
            return fix2.Distance(Position.xz, prevPosition.xz) / (_config.speed * deltaTime);
        }

        private void DoPreAttack(fix deltaTime)
        {
            fix prevPreAttack = PreAttack * _config.preAttack;
            fix newPreAttack = Maths.MoveTowards(prevPreAttack, _config.preAttack, deltaTime);
            PreAttack = newPreAttack / _config.preAttack;
        }
        private void DoPostAttack(fix deltaTime)
        {
            fix prevPostAttack = PostAttack * _config.postAttack;
            fix newPostAttack = Maths.MoveTowards(prevPostAttack, _config.postAttack, deltaTime);
            PostAttack = newPostAttack / _config.postAttack;
        }
    }
}
