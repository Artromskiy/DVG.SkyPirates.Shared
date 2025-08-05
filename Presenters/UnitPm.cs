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
        public float3 TargetPosition { get; set; }
        public float TargetRotation { get; set; }

        private float3 _position;
        private float _rotation;

        public UnitMemento GetMemento() => new UnitMemento(_position, _rotation, 0, 0);

        public void SetMemento(UnitMemento value)
        {
            _position = value.Position;
            _rotation = value.Rotation;
        }

        public UnitPm(IUnitView view, UnitModel model) : base(view, model) { }

        public void Tick(float deltaTime)
        {
            Move(deltaTime);
            View.Position = _position;
            View.Rotation = _rotation;
        }

        private void Move(float deltaTime)
        {
            var direction = TargetPosition.xz - _position.xz;
            _position = float3.MoveTowards(_position, TargetPosition, Model.speed * deltaTime);
            if (float2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                _rotation = Maths.RotateTowards(_rotation, rotation, 720 * deltaTime);
            }
        }
    }
}
