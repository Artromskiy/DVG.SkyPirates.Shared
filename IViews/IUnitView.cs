using DVG.Core;

namespace DVG.SkyPirates.Shared.IViews
{
    public interface IUnitView : IView
    {
        public float2 Velocity { set; }
        public float Rotation { set; }

        public float3 Position { get; }
    }
}
