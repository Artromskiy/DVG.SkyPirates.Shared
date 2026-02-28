using DVG.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command(false)]
    public struct InvalidateCommand
    {
        public int CommandId;
    }
}
