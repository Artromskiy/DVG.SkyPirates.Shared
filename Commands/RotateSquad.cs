namespace DVG.SkyPirates.Shared.Commands
{
    public readonly struct RotateSquad
    {
        public readonly float rotation;

        public RotateSquad(float rotation)
        {
            this.rotation = rotation;
        }
    }
}
