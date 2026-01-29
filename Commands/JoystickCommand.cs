using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct JoystickCommand
    {
        [DataMember(Order = 0)]
        public fix2 Direction { get; set; }
        [DataMember(Order = 1)]
        public bool Fixation { get; set; }

        public JoystickCommand(fix2 direction, bool fixation)
        {
            Direction = direction;
            Fixation = fixation;
        }
    }
}
