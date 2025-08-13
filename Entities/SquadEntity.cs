using DVG.Core;
using DVG.SkyPirates.Shared.ICommandables;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Mementos;
using DVG.SkyPirates.Shared.Configs;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Entities
{
    public class SquadEntity :
        IFixedTickable,
        IRotationable,
        IDirectionable,
        IFixationable,
        IMementoable<SquadMemento>
    {
        public fix3 Position { get; private set; }
        public fix Rotation { get; private set; }

        public bool Fixation;
        private fix2 _direction;

        private readonly List<int> _units = new List<int>();
        private int[] _order = Array.Empty<int>();
        private fix2[] _rotatedPoints;

        public int UnitsCount => _units.Count;

        private PackedCirclesConfig _packedCircles;
        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
        private readonly IEntitiesService _entitiesService;

        public SquadEntity(IPathFactory<PackedCirclesConfig> circlesModelFactory, IEntitiesService entitiesService)
        {
            _entitiesService = entitiesService;
            _circlesModelFactory = circlesModelFactory;
            _packedCircles = _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + 1);
            _rotatedPoints = new fix2[1] { fix2.zero };
            _order = new int[1] { 0 };
        }

        public void AddUnit(int unitEntityId)
        {
            UpdatePackedCircles(_units.Count + 1);
            _units.Add(unitEntityId);
        }

        public void RemoveUnit(int unitEntityId)
        {
            _units.Remove(unitEntityId);
        }

        private void UpdatePackedCircles(int count)
        {
            _packedCircles = _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
            Array.Resize(ref _order, count);
            _order[count - 1] = count - 1;
            UpdateRotatedPoints();
        }

        public void Tick(fix deltaTime)
        {
            var deltaMove = (deltaTime * _direction * 7).x_y;
            Position += deltaMove;
            for (int i = 0; i < _units.Count; i++)
            {
                var unit = _entitiesService.GetEntity<UnitEntity>(_units[i]);
                var offset = _rotatedPoints[_order[i]].x_y;
                unit.TargetPosition = Position + offset;
            }
        }

        public void SetRotation(fix rotation)
        {
            var newQuantizedRotation = (int)Maths.Round(rotation * 16 / 360);
            var oldQuantizedRotation = (int)Maths.Round(Rotation * 16 / 360);
            int deltaRotation = newQuantizedRotation - oldQuantizedRotation;
            deltaRotation = deltaRotation < 0 ? deltaRotation + 16 : deltaRotation;
            if (deltaRotation == 0)
                return;

            Rotation = newQuantizedRotation * 360 / 16;
            int[] newOrder = new int[_order.Length];
            for (int i = 0; i < _order.Length; i++)
                newOrder[i] = _packedCircles.Reorders[deltaRotation, _order[i]];
            _order = newOrder;

            UpdateRotatedPoints();
        }

        private void UpdateRotatedPoints()
        {
            Array.Resize(ref _rotatedPoints, _packedCircles.Points.Length);
            var radians = Maths.Radians(Rotation);
            for (int i = 0; i < _packedCircles.Points.Length; i++)
            {
                var localPoint = _packedCircles.Points[i] / 2;
                _rotatedPoints[i] = RotatePoint(localPoint, radians);
            }
        }

        public void SetDirection(fix2 direction)
        {
            _direction = direction;
        }
        public void SetFixation(bool fixation) { }

        public SquadMemento GetMemento()
        {
            return new SquadMemento(Position, Rotation, _direction, Fixation, _units.ToArray(), _order);
        }

        public void SetMemento(SquadMemento memento)
        {
            Position = memento.Position;
            Rotation = memento.Rotation;
            Fixation = memento.Fixation;
            _units.Clear();
            _units.AddRange(memento.UnitsIds);
            _order = memento.Order;
        }

        public static fix2 RotatePoint(fix2 vec, fix radians)
        {
            var cs = Maths.Cos(radians);
            var sn = Maths.Sin(radians);
            fix x = vec.x * cs + vec.y * sn;
            fix y = -vec.x * sn + vec.y * cs;
            return new fix2(x, y);
        }
    }
}
