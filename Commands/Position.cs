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
    public readonly partial struct Position
    {
        [DataMember(Order = 0)]
        public readonly real3 position;

        public Position(real3 position)
        {
            this.position = position;
        }
    }
}
