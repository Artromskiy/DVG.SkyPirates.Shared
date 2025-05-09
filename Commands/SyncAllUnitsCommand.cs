namespace DVG.SkyPirates.Shared.Commands
{
    public struct SyncAllUnitsCommand
    {
        public int[] ids;

        public SyncAllUnitsCommand(int[] ids)
        {
            this.ids = ids;
        }
    }
}
