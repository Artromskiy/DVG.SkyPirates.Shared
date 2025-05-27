using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.ICommandRecievers
{
    public interface IPositionable : ICommandReciever<Position>
    {
        void SetPosition(float3 position);
        void ICommandReciever<Position>.Recieve(Position cmd) => SetPosition(cmd.position);
    }
}
