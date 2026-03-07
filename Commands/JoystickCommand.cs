using DVG.Commands.Attributes;
using DVG.Components;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command(true)]
    public struct JoystickCommand
    {
        public SyncId Target;
        public fix2 Direction;
        public bool Fixation;
    }
}
