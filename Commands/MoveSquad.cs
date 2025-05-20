namespace DVG.SkyPirates.Shared.Commands
{
    public readonly struct MoveSquad
    {
        public readonly float3 position;

        public MoveSquad(float3 position)
        {
            this.position = position;
        }
    }
}
