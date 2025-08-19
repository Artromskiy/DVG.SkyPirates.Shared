using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    public class SquadSystem : ITickableExecutor
    {
        private readonly World _world;

        public SquadSystem(World world)
        {
            _world = world;
        }

        public void Tick(fix deltaTime)
        {
            _world.Query(new SquadArch(), (ref Position p, ref Direction d, ref Squad s, ref Rotation r, ref Fixation f) =>
            {
                var deltaMove = (d.direction * 7 * deltaTime).x_y;
                p.position += deltaMove;

                for (int i = 0; i < s.units.Count; i++)
                {
                    var offset = s.positions[s.orders[i]].x_y;
                    _world.Get<Unit>(s.units[i]).TargetPosition = p.position + offset;
                    _world.Get<Fixation>(s.units[i]).fixation = f.fixation;
                }
            });
        }
    }
}
