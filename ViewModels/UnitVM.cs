using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IViewModels;

namespace DVG.SkyPirates.Shared.ViewModels
{
    public class UnitVM : IUnitVM
    {
        private readonly UnitEntity _unit;
        public float3 Position => _unit.Position;
        public float Rotation => _unit.Rotation;
        public float PreAttack => _unit.PreAttack;
        public float PostAttack => _unit.PostAttack;

        public UnitVM(UnitEntity unit)
        {
            _unit = unit;
        }

    }
}
