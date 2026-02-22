using DVG.Components;
using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public partial struct JoystickCommand
    {
        public SyncId Target;
        public fix2 Direction;
        public bool Fixation;
    }
}
