using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandRecievers
{
    public interface IPositionable : ICommandable<Position>
    {
        void SetPosition(float3 position);
        void ICommandable<Position>.Recieve(Position cmd) => SetPosition(cmd.position);
    }
}
