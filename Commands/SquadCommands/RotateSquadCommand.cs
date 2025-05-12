namespace DVG.SkyPirates.Shared.Commands.SquadCommands
{
    public readonly struct RotateSquadCommand
    {
        public readonly int squadId;

        public readonly float rotation;

        public RotateSquadCommand(int squadId, float rotation)
        {
            this.squadId = squadId;
            this.rotation = rotation;
        }
    }
}
