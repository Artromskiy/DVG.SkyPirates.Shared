using DVG.Core;
using DVG.SkyPirates.Shared.Models;
using System;
using System.Collections.Generic;

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
            Reorder(_packedCircles.points, _order, _rotation, newRotation);
            _rotation = newRotation;
            Console.WriteLine(string.Join(", ", _order));
        }

        private static void Reorder(float2[] positions, int[] order, float oldRotation, float newRotation)
        {
            int count = positions.Length;
            for (int i = 0; i < count; i++)
            {
                var posI = RotatePoint(positions[order[i]], Maths.Radians(newRotation));
                var oldPosI = RotatePoint(positions[order[i]], Maths.Radians(oldRotation));
                float firstDist = float2.Distance(posI, oldPosI);
                if (firstDist < MinSwapDistance)
                    continue;

                int swapIndex = -1;
                float minSwap = float.MaxValue;
                for (int j = i + 1; j < count; j++)
                {
                    var posJ = RotatePoint(positions[order[j]], Maths.Radians(newRotation));
                    var oldPosJ = RotatePoint(positions[order[j]], Maths.Radians(oldRotation));
                    var swapSum = float2.Distance(posI, oldPosJ) + float2.Distance(posJ, oldPosI);
                    if (swapSum < minSwap &&
                        swapSum < firstDist + float2.Distance(posJ, oldPosJ)) // prevent nearly equal swap
                    {
                        minSwap = swapSum;
                        swapIndex = j;
                    }
                }
                if (swapIndex != -1)
                {
                    (order[swapIndex], order[i]) = (order[i], order[swapIndex]);
                }
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
