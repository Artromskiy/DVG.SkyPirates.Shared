using DVG.Core;
using DVG.SkyPirates.Shared.Models;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Presenters
{
    internal class SquadPm : Presenter, ITickable
    {
        private const float MinSwapDistance = 0.01f;
        public float3 Position;
        public bool Fixation;

        private readonly List<UnitPm> _units = new List<UnitPm>();
        private PackedCirclesModel _packedCircles;

        public int UnitsCount => _units.Count;

        private readonly IPathFactory<PackedCirclesModel> _circlesModelFactory;
        private int[] _order = Array.Empty<int>();

        private int _quantizedRotation = 0;
        private float _rotation = 0;

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
                _units[i].TargetPosition = Position + RotatePoint(_packedCircles.points[_order[i]] * 0.5f, Maths.Radians(_rotation)).x_y;

            foreach (var item in _units)
                item.Tick(deltaTime);
        }

        public void Rotate(float rotation)
        {
            var newQuantizedRotation = (int)Maths.Round(rotation * 16 / 360);
            if (_quantizedRotation == newQuantizedRotation)
                return;
            _quantizedRotation = newQuantizedRotation;
            var newRotation = _quantizedRotation * 360 / 16;
            _order = GetOrder(_packedCircles.points, _order, _rotation, newRotation);
            //Console.WriteLine(string.Join(", ", _order));
            _rotation = newRotation;
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

        public static float2 RotatePoint(float2 vec, float radians)
        {
            var cs = Maths.Cos(radians);
            var sn = Maths.Sin(radians);
            float x = vec.x * cs + vec.y * sn;
            float y = -vec.x * sn + vec.y * cs;
            return new float2(x, y);
        }
    }
}
