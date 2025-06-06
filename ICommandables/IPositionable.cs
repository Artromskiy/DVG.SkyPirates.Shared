using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IPositionable : ICommandable<Position>
    {
        void SetPosition(float3 position);
        void ICommandable<Position>.Execute(Position cmd) => SetPosition(cmd.position);
    }
}
