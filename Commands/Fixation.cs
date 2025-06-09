using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct Fixation
    {
        [DataMember(Order = 0)]
        public readonly bool fixation;

        public Fixation(bool fixation)
        {
            this.fixation = fixation;
        }
    }
}
