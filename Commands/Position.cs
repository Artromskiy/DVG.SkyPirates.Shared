using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct Position
    {
        [DataMember(Order = 0)]
        public readonly float3 position;

        public Position(float3 position)
        {
            this.position = position;
        }
    }
}
