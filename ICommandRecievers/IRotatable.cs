using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.ICommandRecievers
{
    public interface IRotatable : ICommandReciever<Rotation>
    {
        void SetRotation(float rotation);
        void ICommandReciever<Rotation>.Recieve(Rotation cmd) => SetRotation(cmd.rotation);
    }
}
