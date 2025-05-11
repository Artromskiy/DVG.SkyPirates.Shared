namespace DVG.SkyPirates.Shared.Commands
{
    public struct UnregisterSquadUnitCommand
    {
        public int id;

        public UnregisterSquadUnitCommand(int id)
        {
            this.id = id;
        }
    }
}
