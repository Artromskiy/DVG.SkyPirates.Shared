namespace DVG.SkyPirates.Shared.Commands.SquadCommands
{
    public readonly struct RegisterSquadCommand
    {
        public readonly int squadId;

        public RegisterSquadCommand(int squadId)
        {
            this.squadId = squadId;
        }
    }
}
