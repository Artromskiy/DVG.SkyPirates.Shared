using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Extensions;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    /// <summary>
    /// Moves Entity's <see href="Position"/> and <see href="Rotation"/> 
    /// with speed <see href="MoveSpeed"/> towards <see href="Destination"/>
    /// </summary>
    public class MoveSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Position, Rotation, Destination, MoveSpeed>();
        private readonly World _world;
        private const int RotateSpeed = 720;
        private readonly HashSet<fix3> _positions = new HashSet<fix3>();
        private readonly HashSet<fix3> _invalidPositions = new HashSet<fix3>();

        public MoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _positions.Clear();
            _invalidPositions.Clear();
            var query = new MoveQuery(_positions, _invalidPositions, deltaTime);
            _world.InlineQuery<MoveQuery, Position, TempPosition, Destination, MoveSpeed>(_desc, ref query);
            _world.InlineQuery<MoveQuery, Position, TempPosition, Rotation, Destination>(_desc, ref query);
        }

        private readonly struct MoveQuery :

            IForEach<Position, TempPosition, Destination, MoveSpeed>,
            IForEach<Position, TempPosition, Rotation, Destination>
        {
            private readonly HashSet<fix3> _positions;
            private readonly HashSet<fix3> _invalidPositions;
            private readonly fix _deltaTime;

            public MoveQuery(HashSet<fix3> positions, HashSet<fix3> invalidPositions, fix deltaTime)
            {
                _positions = positions;
                _invalidPositions = invalidPositions;
                _deltaTime = deltaTime;
            }

            public void Update(ref Position position, ref TempPosition tempPosition, ref Destination destination, ref MoveSpeed moveSpeed)
            {
                var pos = fix3.MoveTowards(
                    position.Value,
                    destination.Position,
                    moveSpeed.Value * _deltaTime);

                tempPosition.Value = pos;

                if (!_positions.Add(pos))
                    _invalidPositions.Add(pos);
            }

            public void Update(ref Position position, ref TempPosition tempPosition, ref Rotation rotation, ref Destination destination)
            {
                fix3 targetPos = _invalidPositions.Contains(tempPosition.Value) ? 
                    position.Value : tempPosition.Value;

                var dir = targetPos.xz - position.Value.xz;
                var rotateTo = fix2.SqrLength(dir) != 0
                    ? Maths.Degrees(MathsExtensions.GetRotation(dir))
                    : destination.Rotation;

                rotation.Value = Maths.RotateTowards(
                    rotation.Value,
                    rotateTo,
                    RotateSpeed * _deltaTime);

                if (_invalidPositions.Contains(tempPosition.Value))
                    return;

                position.Value = targetPos;
            }
        }
    }
}
