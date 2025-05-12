namespace DVG.SkyPirates.Shared.Commands.SquadCommands
{
    public readonly struct MoveSquadCommand
    {
        public readonly int squadId;

        public readonly float3 position;

        public MoveSquadCommand(int squadId, float3 position)
        {
            this.squadId = squadId;
            this.position = position;
        }
    }
}
