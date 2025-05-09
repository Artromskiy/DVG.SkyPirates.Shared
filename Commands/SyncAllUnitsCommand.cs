namespace DVG.SkyPirates.Shared.Commands
{
    public struct SyncAllUnitsCommand
    {
        public uint[] ids;

        public SyncAllUnitsCommand(uint[] ids)
        {
            this.ids = ids;
        }
    }
}
