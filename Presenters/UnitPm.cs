#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;
using DVG.SkyPirates.Shared.IViews;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Shared.Presenters
{
    public class UnitPm :
        Presenter<IUnitView, UnitModel>,
        IMementoable<UnitMemento>,
        ITickable
    {
        public real3 TargetPosition { get; set; }
        public real TargetRotation { get; set; }

        private real3 _position;
        private real _rotation;

        public UnitMemento GetMemento() => new UnitMemento(_position, _rotation, 0, 0);

        public void SetMemento(UnitMemento value)
        {
            _position = value.Position;
            _rotation = value.Rotation;
        }

        public UnitPm(IUnitView view, UnitModel model) : base(view, model) { }

        public void Tick(real deltaTime)
        {
            Move(deltaTime);
            View.Position = _position;
            View.Rotation = _rotation;
        }

        private void Move(real deltaTime)
        {
            var direction = TargetPosition.xz - _position.xz;
            _position = real3.MoveTowards(_position, TargetPosition, Model.speed * deltaTime);
            if (real2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                _rotation = Maths.RotateTowards(_rotation, rotation, 720 * deltaTime);
            }
        }
    }
}
