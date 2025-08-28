﻿using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Extensions;

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
        public MoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new MoveQuery(deltaTime);
            _world.InlineQuery<MoveQuery, Position, Rotation, Destination, MoveSpeed>(_desc, ref query);
        }

        private readonly struct MoveQuery :
            IForEach<Position, Rotation, Destination, MoveSpeed>
        {
            private readonly fix _deltaTime;

            public MoveQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Position position, ref Rotation rotation, ref Destination destination, ref MoveSpeed moveSpeed)
            {
                MoveTowardsDestination(ref position, destination, moveSpeed);
                RotateTowardsDestination(ref rotation, position, destination);
            }

            private void MoveTowardsDestination(ref Position position, Destination destination, MoveSpeed moveSpeed)
            {
                position.Value = fix3.MoveTowards(
                    position.Value,
                    destination.Position,
                    moveSpeed.Value * _deltaTime);
            }

            private void RotateTowardsDestination(ref Rotation rotation, Position position, Destination destination)
            {
                var dir = destination.Position.xz - position.Value.xz;
                var rotateTo = fix2.SqrLength(dir) != 0
                    ? Maths.Degrees(MathsExtensions.GetRotation(dir))
                    : destination.Rotation;

                rotation.Value = Maths.RotateTowards(
                    rotation.Value,
                    rotateTo,
                    RotateSpeed * _deltaTime);
            }
        }
    }
}
