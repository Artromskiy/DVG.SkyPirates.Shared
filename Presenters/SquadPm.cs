using DVG.Core;
using DVG.SkyPirates.Shared.Models;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Presenters
{
    internal class SquadPm : Presenter, ITickable
    {
        public float3 Position;
        public float Rotation;
        public bool Fixation;

        private readonly List<UnitPm> _units = new List<UnitPm>();
        private PackedCirclesModel _packedCircles;

        public int UnitsCount => _units.Count;

        private readonly IPathFactory<PackedCirclesModel> _circlesModelFactory;

        private float3[] _targetPositions = Array.Empty<float3>();

        public SquadPm(IPathFactory<PackedCirclesModel> circlesModelFactory)
        {
            _circlesModelFactory = circlesModelFactory;
            UpdatePackedCircles(1);
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
            _targetPositions = new float3[count];
        }

        public void Tick()
        {
            for (int i = 0; i < _targetPositions.Length; i++)
                _targetPositions[i] = Position + rotate(_packedCircles.points[i] * 0.5f, Maths.Radians(Rotation)).x_y;

            ReorderUnitsToNearest(_targetPositions);
            for (int i = 0; i < _units.Count; i++)
            {
                _units[i].TargetPosition = _targetPositions[i];
            }

            foreach (var item in _units)
                item.Tick();
        }

        private void ReorderUnitsToNearest(float3[] targets)
        {
            int count = Maths.Min(targets.Length, _units.Count);
            for (int i = 0; i < count; i++)
            {
                var posI = _units[i].View.Position;
                float firstDist = float2.SqrDistance(posI.xz, targets[i].xz);

                int swapIndex = -1;
                float minSwap = float.MaxValue;
                for (int j = i + 1; j < count; j++)
                {
                    var posJ = _units[j].View.Position;
                    var swapSum = float2.SqrDistance(posI.xz, targets[j].xz) + float2.SqrDistance(posJ.xz, targets[i].xz);
                    if (swapSum < minSwap && swapSum < firstDist + float2.SqrDistance(posJ.xz, targets[j].xz))
                    {
                        minSwap = swapSum;
                        swapIndex = j;
                    }
                }
                if (swapIndex != -1)
                    (_units[swapIndex], _units[i]) = (_units[i], _units[swapIndex]);
            }
        }

        public static float2 rotate(float2 vec, float radians)
        {
            var cs = Maths.Cos(radians);
            var sn = Maths.Sin(radians);
            float x = vec.x * cs + vec.y * sn;
            float y = -vec.x * sn + vec.y * cs;
            return new float2(x, y);
        }
    }
}
