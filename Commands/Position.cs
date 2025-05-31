using DVG.Core;
using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly struct Position: ICommandData
    {
        public readonly float3 position;

        public Position(float3 position)
        {
            this.position = position;
        }

        public int CommandId => throw new System.NotImplementedException();
    }
}
