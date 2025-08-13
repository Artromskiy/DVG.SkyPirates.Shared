using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct Direction
    {
        [DataMember(Order = 0)]
        public readonly fix2 direction;

        public Direction(fix2 direction)
        {
            this.direction = direction;
        }
    }
}
