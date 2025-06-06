using DVG.Core;
using DVG.SkyPirates.Shared.ICommandables;
using DVG.SkyPirates.Shared.Mementos;
using DVG.SkyPirates.Shared.Models;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Presenters
{
    public class SquadPm : Presenter,
        ITickable,
        IRotationable,
        IPositionable,
        IFixationable,
        IMementoable<SquadMemento>
    {
        private const float MinSwapDistance = 0.01f;

        private float3 Position;
        private float Rotation;
        public bool Fixation;
        private readonly List<UnitPm> _units = new List<UnitPm>();
        private int[] _order = Array.Empty<int>();


        public int UnitsCount => _units.Count;

        private PackedCirclesModel _packedCircles;
        private readonly IPathFactory<PackedCirclesModel> _circlesModelFactory;


        public SquadPm(IPathFactory<PackedCirclesModel> circlesModelFactory)
        {
            _circlesModelFactory = circlesModelFactory;
            _packedCircles = _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + 1);
            _order = new int[1] { 0 };
        }

        public void AddUnit(UnitPm unit, int unitId)
        {
            UpdatePackedCircles(_units.Count + 1);
            _units.Add(unit);
        }

        public void RemoveUnit(int unitId)
        {
            _units.RemoveAt(unitId);
        }

        private void UpdatePackedCircles(int count)
        {
            _packedCircles = _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
            Array.Resize(ref _order, count);
            _order[count - 1] = count - 1;
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < _units.Count; i++)
                _units[i].TargetPosition = Position + RotatePoint(_packedCircles.points[_order[i]] * 0.5f, Maths.Radians(Rotation)).x_y;

            foreach (var item in _units)
                item.Tick(deltaTime);
        }

        public void SetRotation(float rotation)
        {
            var newQuantizedRotation = (int)Maths.Round(rotation * 16 / 360);
            var oldQuantizedRotation = (int)Maths.Round(Rotation * 16 / 360);
            if (oldQuantizedRotation == newQuantizedRotation)
                return;
            var newRotation = newQuantizedRotation * 360 / 16;
            _order = GetOrder(_packedCircles.points, _order, Rotation, newRotation);
            Rotation = newRotation;

            Console.WriteLine(string.Join(", ", _order));
        }

        public void SetPosition(float3 position) => Position = position;
        public void SetFixation(bool fixation) { }

        public SquadMemento GetMemento()
        {
            throw new NotImplementedException();
        }

        public void SetMemento(SquadMemento memento)
        {
            throw new NotImplementedException();
        }


        public static float2 RotatePoint(float2 vec, float radians)
        {
            var cs = Maths.Cos(radians);
            var sn = Maths.Sin(radians);
            float x = vec.x * cs + vec.y * sn;
            float y = -vec.x * sn + vec.y * cs;
            return new float2(x, y);
        }

        private static int[] GetOrder(float2[] points, int[] oldOrder, float oldRotation, float newRotation)
        {
            int length = points.Length;
            int[] order = new int[length];
            for (int i = 0; i < length; i++)
                order[i] = i;
            var newRad = Maths.Radians(newRotation);
            var oldRad = Maths.Radians(oldRotation);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    int oi = order[i];
                    int oj = order[j];

                    float2 lhs = RotatePoint(points[oi], newRad);
                    float2 rhs = RotatePoint(points[oj], newRad);

                    float2 oldLhs = RotatePoint(points[oldOrder[i]], oldRad);
                    float2 oldRhs = RotatePoint(points[oldOrder[j]], oldRad);

                    float beforeDist =
                        float2.Distance(lhs, oldLhs) +
                        float2.Distance(rhs, oldRhs);

                    float afterDist =
                        float2.Distance(rhs, oldLhs) +
                        float2.Distance(lhs, oldRhs);

                    if (beforeDist - afterDist > MinSwapDistance)
                        (order[i], order[j]) = (order[j], order[i]);
                }
            }

            return order;
        }
    }
}
