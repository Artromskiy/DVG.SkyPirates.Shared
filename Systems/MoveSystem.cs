using Delta;
using Delta.ECS;
using DVG.SkyPirates.Shared.Ecs;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Extensions;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Moves Entity's <see href="Position"/> and <see href="Rotation"/> 
    /// with speed <see href="MoveSpeed"/> towards <see href="Destination"/>
    /// </summary>
    public sealed class MoveSystem : IDeltaTickableExecutor
    {
        private Query? _descCache;
        private Query _desc => _descCache ??= _world.
            WhereAll<Position, Rotation, Destination, MaxSpeed>().Alive().NotDisabled();

        private readonly World _world;
        private const int RotateSpeed = 720;
        public MoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var delta = deltaTime;
            var desc = _desc;
            _world.ForEach<fix, Position, Rotation, Destination, MaxSpeed>(in desc, ref delta,
                static (ref fix deltaTime, ref Position position, ref Rotation rotation, ref Destination destination, ref MaxSpeed moveSpeed) =>
                {
                    position = fix3.MoveTowards(position, destination.Position, moveSpeed * deltaTime);

                    var dir = destination.Position.xz - ((fix3)position).xz;
                    var rotateTo = fix2.SqrLength(dir) != 0
                        ? Maths.Degrees(MathsExtensions.GetRotation(dir))
                        : destination.Rotation;

                    rotation = Maths.RotateTowards(rotation, rotateTo, RotateSpeed * deltaTime);
                });
        }
    }
}
