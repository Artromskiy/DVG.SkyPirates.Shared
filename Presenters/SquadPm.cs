using DVG.Core;
using DVG.SkyPirates.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Presenters
{
    internal class SquadPm : Presenter, ITickable
    {
        const float MinSwapDistance = 0.1f;
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

        public void Tick()
        {
            for (int i = 0; i < _units.Count; i++)
            {
                _units[i].TargetPosition = Position + RotatePoint(_packedCircles.points[_order[i]] * 0.5f, Maths.Radians(_rotation)).x_y;
            }

            foreach (var item in _units)
                item.Tick();
        }

        public void Rotate(float rotation)
        {
            var newQuantizedRotation = (int)Maths.Round(rotation * 16 / 360);
            if (_quantizedRotation == newQuantizedRotation)
                return;
            _quantizedRotation = newQuantizedRotation;
            var newRotation = _quantizedRotation * 360 / 16;
            var newOrder = Enumerable.Range(0, _order.Length).ToArray();
            Reorder(_packedCircles.points, _order, newOrder, _rotation, newRotation);
            _order = newOrder;
            _rotation = newRotation;
            Console.WriteLine(string.Join(", ", _order));
        }

        private static void Reorder(float2[] positions, int[] oldOrder, int[] newOrder, float oldRotation, float newRotation)
        {
            int count = positions.Length;
            for (int i = 0; i < count; i++)
            {
                var posI = RotatePoint(positions[newOrder[i]], Maths.Radians(newRotation));
                var posI2 = RotatePoint(positions[oldOrder[i]], Maths.Radians(oldRotation));
                float firstDist = float2.SqrDistance(posI, posI2);
                int swapIndex = -1;
                float minSwap = float.MaxValue;
                for (int j = i + 1; j < count; j++)
                {
                    var posJ = RotatePoint(positions[newOrder[j]], Maths.Radians(newRotation));
                    var posJ2 = RotatePoint(positions[oldOrder[j]], Maths.Radians(oldRotation));
                    var swapSum = float2.SqrDistance(posI, posJ2) + float2.SqrDistance(posJ, posI2);
                    if (swapSum < minSwap && swapSum < firstDist + float2.SqrDistance(posJ, posJ2))
                    {
                        minSwap = swapSum;
                        swapIndex = j;
                    }
                }
                if (swapIndex != -1)
                    (newOrder[swapIndex], newOrder[i]) = (newOrder[i], newOrder[swapIndex]);
            }
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
