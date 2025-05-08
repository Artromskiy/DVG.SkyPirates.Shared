
namespace DVG.SkyPirates.Shared.Messages
{
    public struct InputGhost
    {
        public float3 position;
        public float rotation;
        public bool fixation;
    }

    public struct SpawnUnit
    {
        public int id;
    }
}
