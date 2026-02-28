using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public struct InvalidateCommand
    {
        public int CommandId;
    }
}
