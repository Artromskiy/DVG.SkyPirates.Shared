using DVG.Core;

namespace DVG.SkyPirates.Shared.IViewModels
{
    public interface IUnitVM : IViewModel
    {
        public fix3 Position { get; }
        public fix Rotation { get; }
        public fix PreAttack { get; }
        public fix PostAttack { get; }
    }
}
