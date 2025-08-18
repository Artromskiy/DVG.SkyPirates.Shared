using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct DirectionCommand
    {
        [DataMember(Order = 0)]
        public fix2 Direction { get; set; }

        public DirectionCommand(fix2 direction)
        {
            Direction = direction;
        }
    }
}
