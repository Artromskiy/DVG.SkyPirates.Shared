using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IRotationable : ICommandable<Rotation>
    {
        void SetRotation(fix rotation);
        void ICommandable<Rotation>.Execute(Rotation cmd) => SetRotation(cmd.rotation);
    }
}
