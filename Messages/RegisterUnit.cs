using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Shared.Messages
{
    public struct SyncAllUnits
    {
        public uint[] ids;

        public SyncAllUnits(uint[] ids)
        {
            this.ids = ids;
        }
    }

    public struct RegisterUnit
    {
        public uint id;

        public RegisterUnit(uint id)
        {
            this.id = id;
        }
    }

    public struct UnitUnregister
    {
        public uint id;

        public UnitUnregister(uint id)
        {
            this.id = id;
        }
    }

    public struct UnitGhost
    {
        public uint id;
        public float3 position;
        public float rotation;

        public UnitGhost(uint id, IUnitView unitView)
        {
            this.id = id;
            position = unitView.Position;
            rotation = unitView.Rotation;
        }
    }
}
