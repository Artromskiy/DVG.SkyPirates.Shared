namespace DVG.SkyPirates.Shared.Commands
{
    public struct SyncAllSquadUnitsCommand
    {
        public int[] ids;

        public SyncAllSquadUnitsCommand(int[] ids)
        {
            this.ids = ids;
        }
    }
}
