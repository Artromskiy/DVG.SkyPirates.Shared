using DVG.Core.Ids.Attributes;

namespace DVG.SkyPirates.Shared.Ids
{
    [StringId]
    [StringIdDrawer(DrawerType.Editor)]
    [StringIdEditorConfig]
    public partial struct UnitId
    {
        public static partial class Constants
        {
            public static readonly UnitId Sailor = new UnitId("Sailor");
            public static readonly UnitId Pirate = new UnitId("Pirate");
            public static readonly UnitId Buccaneer = new UnitId("Buccaneer");
            public static readonly UnitId Rogue = new UnitId("Rogue");
            public static readonly UnitId Skelly = new UnitId("Skelly");
            public static readonly UnitId Archy = new UnitId("Archy");
            public static readonly UnitId Demon = new UnitId("Demon");
            public static readonly UnitId Imp = new UnitId("Imp");
            public static readonly UnitId Militia = new UnitId("Militia");

            public static readonly UnitId[] AllIds = new UnitId[]
            {
                new UnitId("Sailor"),
                new UnitId("Pirate"),
                new UnitId("Buccaneer"),
                new UnitId("Rogue"),
                new UnitId("Skelly"),
                new UnitId("Archy"),
                new UnitId("Demon"),
                new UnitId("Imp"),
                new UnitId("Militia"),
            };
        }
    }
}
