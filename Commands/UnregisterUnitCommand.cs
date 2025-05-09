namespace DVG.SkyPirates.Shared.Commands
{
    public struct UnregisterUnitCommand
    {
        public uint id;

        public UnregisterUnitCommand(uint id)
        {
            this.id = id;
        }
    }
}
