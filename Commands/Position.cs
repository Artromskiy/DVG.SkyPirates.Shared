using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly partial struct Position
    {
        public readonly float3 position;

        public Position(float3 position)
        {
            this.position = position;
        }
    }
}
