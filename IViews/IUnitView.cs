using DVG.Core;

namespace DVG.SkyPirates.Shared.IViews
{
    public interface IUnitView : IView
    {
        public float3 Position { get; set; }
        public float Rotation { get; set; }
        public float PreAttack { get; set; }
        public float PostAttack { get; set; }
    }
}
