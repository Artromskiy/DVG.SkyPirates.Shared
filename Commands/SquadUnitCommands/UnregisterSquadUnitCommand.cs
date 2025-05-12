namespace DVG.SkyPirates.Shared.Commands.SquadUnitCommands
{
    public struct UnregisterSquadUnitCommand
    {
        public int squadId;
        public int unitId;

        public UnregisterSquadUnitCommand(int squadId, int unitId)
        {
            this.squadId = squadId;
            this.unitId = unitId;
        }
    }
}
