using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Shared.Commands
{
    public struct UpdateUnitCommand
    {
        public int id;
        public float3 position;
        public float rotation;

        public UpdateUnitCommand(int id, IUnitView unitView)
        {
            this.id = id;
            position = unitView.Position;
            rotation = unitView.Rotation;
        }
    }
}
