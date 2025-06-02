using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandRecievers
{
    public interface IRotatable : ICommandable<Rotation>
    {
        void SetRotation(float rotation);
        void ICommandable<Rotation>.Recieve(Rotation cmd) => SetRotation(cmd.rotation);
    }
}
