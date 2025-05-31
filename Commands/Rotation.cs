using DVG.Core;
using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly struct Rotation: ICommandData
    {
        public readonly float rotation;

        public Rotation(float rotation)
        {
            this.rotation = rotation;
        }

        public int CommandId => throw new System.NotImplementedException();
    }
}
