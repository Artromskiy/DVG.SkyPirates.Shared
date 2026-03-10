using DVG.Commands.Attributes;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command(true)]
    public struct SetEntityDataCommand
    {
        public SyncId Target;
        public ComponentsSet Components;
    }
}
