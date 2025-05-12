namespace DVG.SkyPirates.Shared.Commands.SquadCommands
{
    public readonly struct FixateSquadCommand
    {
        public readonly int squadId;

        public readonly bool fixation;

        public FixateSquadCommand(int squadId, bool fixation)
        {
            this.squadId = squadId;
            this.fixation = fixation;
        }
    }
}
