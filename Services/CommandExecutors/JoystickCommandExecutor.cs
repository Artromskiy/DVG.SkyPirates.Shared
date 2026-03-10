using Arch.Core;
using DVG.Commands;
using DVG.Components;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Systems;
using DVG.SkyPirates.Shared.Tools.Extensions;
using DVG.SkyPirates.Shared.Tools.TraceHelpers;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class JoystickCommandExecutor : ICommandExecutor<JoystickCommand>
    {
        private readonly IEntityRegistry _entityRegistryService;
        private readonly World _world;

        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<SquadMember>().NotDisabled().Alive();

        public JoystickCommandExecutor(IEntityRegistry entityRegistryService, World world)
        {
            _entityRegistryService = entityRegistryService;
            _world = world;
        }

        public void Execute(Command<JoystickCommand> cmd)
        {
            _entityRegistryService.TryGet(cmd.Data.Target, out var squad);

            if (squad == Entity.Null ||
                !_world.IsAlive(squad) ||
                !_world.Has<Alive>(squad))
            {
                Trace.TraceWarning(Tracing.NotCreatedEntityCommand(cmd.Data.Target));
                return;
            }

            if (!CanMove(squad))
                return;

            ref var dir = ref _world.Get<Direction>(squad);
            ref var rot = ref _world.Get<Rotation>(squad);
            ref var fix = ref _world.Get<Fixation>(squad);
            dir = cmd.Data.Direction;
            fix = cmd.Data.Fixation;

            if (fix2.SqrLength(dir) == 0)
                return;
            rot = Maths.Degrees(MathsExtensions.GetRotation(dir));
        }

        private bool CanMove(Entity squad)
        {
            var squadId = _world.Get<SyncId>(squad);
            int count = 0;
            _world.Query(in _desc, (ref SquadMember member) =>
            {
                if (member.SquadId == squadId)
                    count++;
            });
            return count > 0;
        }
    }
}
