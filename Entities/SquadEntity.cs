using DVG.Core;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.ICommandables;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Mementos;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Entities
{
    public class SquadEntity :
        IFixedTickable,
        IDirectionable,
        IFixationable,
        IMementoable<SquadMemento>
    {
        public fix3 Position { get; private set; }

        public bool Fixation;
        private fix2 _direction;
        private fix _rotation;

        private readonly List<int> _units = new List<int>();
        private readonly List<int> _orders = new List<int>();
        private fix2[] _rotatedPoints;

        public int UnitsCount => _units.Count;

        private PackedCirclesConfig _packedCircles;
        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
        private readonly IEntitiesService _entitiesService;

        public SquadEntity(IPathFactory<PackedCirclesConfig> circlesModelFactory, IEntitiesService entitiesService)
        {
            _entitiesService = entitiesService;
            _circlesModelFactory = circlesModelFactory;
            _packedCircles = GetCirclesConfig(1);
            _rotatedPoints = new fix2[1] { fix2.zero };
        }

        public void AddUnit(int unitEntityId)
        {
            _packedCircles = GetCirclesConfig(_units.Count + 1);
            _orders.Add(_units.Count);
            _units.Add(unitEntityId);
            UpdateRotatedPoints();
        }

        public void RemoveUnit(int unitEntityId)
        {
            _units.Remove(unitEntityId);
        }

        public void Tick(fix deltaTime)
        {
            var deltaMove = (_direction * 7 * deltaTime).x_y;
            Position += deltaMove;
            for (int i = 0; i < _units.Count; i++)
            {
                var unit = _entitiesService.GetEntity<UnitEntity>(_units[i]);
                var offset = _rotatedPoints[_orders[i]].x_y;
                unit.TargetPosition = Position + offset;
            }
        }


        public void SetDirection(fix2 direction)
        {
            _direction = direction;

            if (fix2.SqrLength(_direction) == 0)
                return;

            var oldRot = _rotation;
            _rotation = GetRotation(_direction);

            static int Quantize(fix a) => (int)Maths.Round(Maths.Degrees(a) * 16 / 360);
            int newQuantizedRotation = Quantize(_rotation);
            int oldQuantizedRotation = Quantize(oldRot);
            int deltaRotation = (newQuantizedRotation - oldQuantizedRotation) & 15;
            if (deltaRotation == 0)
                return;

            for (int i = 0; i < _orders.Count; i++)
                _orders[i] = _packedCircles.Reorders[deltaRotation, _orders[i]];
            UpdateRotatedPoints();
        }


        private void UpdateRotatedPoints()
        {
            Array.Resize(ref _rotatedPoints, _packedCircles.Points.Length);
            for (int i = 0; i < _packedCircles.Points.Length; i++)
            {
                var localPoint = _packedCircles.Points[i] / 2;
                _rotatedPoints[i] = RotatePoint(localPoint, _rotation);
            }
        }

        public void SetFixation(bool fixation) { }

        public SquadMemento GetMemento()
        {
            return new SquadMemento(
                Position,
                _direction,
                _rotation,
                Fixation,
                _units.ToArray(),
                _orders.ToArray());
        }

        public void SetMemento(SquadMemento memento)
        {
            Position = memento.Position;
            Fixation = memento.Fixation;
            _rotation = memento.Rotation;
            _units.Clear();
            _units.AddRange(memento.UnitsIds);
            _orders.Clear();
            _orders.AddRange(memento.Order);
            if (_orders.Count != 0)
                _packedCircles = GetCirclesConfig(_orders.Count);
        }

        private static fix GetRotation(fix2 direction)
        {
            return -Maths.Atan2(-direction.x, direction.y);
        }

        public static fix2 RotatePoint(fix2 vec, fix radians)
        {
            var cs = Maths.Cos(radians);
            var sn = Maths.Sin(radians);
            fix x = vec.x * cs + vec.y * sn;
            fix y = -vec.x * sn + vec.y * cs;
            return new fix2(x, y);
        }

        private PackedCirclesConfig GetCirclesConfig(int count)
        {
            return _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
        }
    }
}
