namespace DVG.SkyPirates.Shared.Commands
{
    public struct UnregisterUnitCommand
    {
        public int id;

        public UnregisterUnitCommand(int id)
        {
            this.id = id;
        }
    }
}
