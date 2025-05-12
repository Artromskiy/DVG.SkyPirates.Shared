using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IViews;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Shared.Presenters
{
    public class UnitPm : Presenter<IUnitView, UnitModel>, ITickable
    {
        public float3 TargetPosition { get; set; }
        public float TargetRotation { get; set; }

        private readonly ITimeProvider _timeProvider;

        public UnitPm(IUnitView view, UnitModel model, ITimeProvider timeProvider) : base(view, model)
        {
            _timeProvider = timeProvider;
        }

        public void Tick()
        {
            Move();
        }

        private void Move()
        {
            var direction = TargetPosition.xz - View.Position.xz;
            View.Position = float3.MoveTowards(View.Position, TargetPosition, Model.speed * _timeProvider.DeltaTime);
            if (float2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                View.Rotation = Maths.RotateTowards(View.Rotation, rotation, 720 * _timeProvider.DeltaTime);
            }
        }
    }
}
