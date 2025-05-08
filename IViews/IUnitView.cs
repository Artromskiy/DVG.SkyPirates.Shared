using DVG.Core;

namespace DVG.SkyPirates.Shared.IViews
{
    public interface IUnitView : IView
    {
        public float Rotation { get; set; }
        public float3 Position { get; set; }
    }
}
