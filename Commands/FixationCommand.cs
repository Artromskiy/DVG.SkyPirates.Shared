using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct FixationCommand
    {
        [DataMember(Order = 0)]
        public bool Fixation { get; set; }

        public FixationCommand(bool fixation)
        {
            this.Fixation = fixation;
        }
    }
}
