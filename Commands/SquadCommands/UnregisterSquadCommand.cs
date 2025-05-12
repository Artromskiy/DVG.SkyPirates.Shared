namespace DVG.SkyPirates.Shared.Commands
{
    public readonly struct UnregisterSquadCommand
    {
        public readonly int squadId;

        public UnregisterSquadCommand(int squadId)
        {
            this.squadId = squadId;
        }
    }
}
