using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct Rotation
    {
        [DataMember(Order = 0)]
        public readonly float rotation;

        public Rotation(float rotation)
        {
            this.rotation = rotation;
        }
    }
}
