using DVG.Core;
using DVG.SkyPirates.Shared.IViews;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Shared.Presenters
{
    public class UnitPm : Presenter<IUnitView, UnitModel>, ITickable
    {
        public float3 TargetPosition { get; set; }
        public float TargetRotation { get; set; }

        public UnitPm(IUnitView view, UnitModel model) : base(view, model) { }

        public void Tick(float deltaTime)
        {
            Move(deltaTime);
        }

        private void Move(float deltaTime)
        {
            var direction = TargetPosition.xz - View.Position.xz;
            View.Position = float3.MoveTowards(View.Position, TargetPosition, Model.speed * deltaTime);
            if (float2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                View.Rotation = Maths.RotateTowards(View.Rotation, rotation, 720 * deltaTime);
            }
        }
    }
}
