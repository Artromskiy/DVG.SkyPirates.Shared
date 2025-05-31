using DVG.Core;
using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly partial struct Rotation
    {
        public readonly float rotation;

        public Rotation(float rotation)
        {
            this.rotation = rotation;
        }
    }
}
