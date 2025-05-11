namespace DVG.SkyPirates.Shared.Commands
{
    public readonly struct RegisterSquadCommand
    {
        public readonly int id;

        public RegisterSquadCommand(int id)
        {
            this.id = id;
        }
    }
}
