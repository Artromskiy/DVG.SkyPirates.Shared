using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly partial struct Fixation
    {
        public readonly bool fixation;

        public Fixation(bool fixation)
        {
            this.fixation = fixation;
        }
    }
}
