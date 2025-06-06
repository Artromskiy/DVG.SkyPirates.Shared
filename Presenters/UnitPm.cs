﻿using DVG.Core;
using DVG.SkyPirates.Shared.IViews;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Shared.Presenters
{
    public class UnitPm :
        Presenter<IUnitView, UnitModel>,
        ITickable,
        IMementoable<UnitMemento>
    {
        public float3 TargetPosition { get; set; }
        public float TargetRotation { get; set; }

        public UnitMemento GetMemento()
        {
            throw new System.NotImplementedException();
        }

        public void SetMemento(UnitMemento value)
        {
            throw new System.NotImplementedException();
        }

        public UnitPm target;

        public UnitPm(IUnitView view, UnitModel model) : base(view, model) { }

        public void Tick(float deltaTime)
        {
            deltaTime = Move(deltaTime);
            deltaTime = Attack(deltaTime);
            deltaTime = Reload(deltaTime);
        }

        private float Move(float deltaTime)
        {
            var direction = TargetPosition.xz - View.Position.xz;
            var travelTime = float2.Length(direction) / Model.speed;
            View.Position = float3.MoveTowards(View.Position, TargetPosition, Model.speed * deltaTime);
            if (float2.SqrLength(direction) != 0)
            {
                var rotation = Maths.Degrees(-Maths.Atan2(-direction.x, direction.y));
                View.Rotation = Maths.RotateTowards(View.Rotation, rotation, 720 * deltaTime);
            }
            return Maths.Max(deltaTime - travelTime, 0);
        }

        private float Attack(float deltaTime)
        {
            return deltaTime;
        }

        private float Reload(float deltaTime)
        {
            return deltaTime;
        }
    }
}
