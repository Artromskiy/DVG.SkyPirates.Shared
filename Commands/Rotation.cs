using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct Rotation
    {
        [DataMember(Order = 0)]
        public readonly fix rotation;

        public Rotation(fix rotation)
        {
            this.rotation = rotation;
        }
    }
}
