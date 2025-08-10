#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct Rotation
    {
        [DataMember(Order = 0)]
        public readonly real rotation;

        public Rotation(real rotation)
        {
            this.rotation = rotation;
        }
    }
}
