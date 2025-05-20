namespace DVG.SkyPirates.Shared.Commands
{
    public readonly partial struct FixateSquad
    {
        public readonly bool fixation;

        public FixateSquad(bool fixation)
        {
            this.fixation = fixation;
        }
    }
}
