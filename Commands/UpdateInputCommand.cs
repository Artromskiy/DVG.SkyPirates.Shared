namespace DVG.SkyPirates.Shared.Commands
{
    public struct UpdateInputCommand
    {
        public float3 position;
        public float rotation;
        public bool fixation;
    }

    public struct SpawnUnitCommand
    {
        public int id;
    }
}
