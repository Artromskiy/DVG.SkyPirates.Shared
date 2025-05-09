using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Shared.Commands
{
    public struct UpdateUnitCommand
    {
        public uint id;
        public float3 position;
        public float rotation;

        public UpdateUnitCommand(uint id, IUnitView unitView)
        {
            this.id = id;
            position = unitView.Position;
            rotation = unitView.Rotation;
        }
    }
}
