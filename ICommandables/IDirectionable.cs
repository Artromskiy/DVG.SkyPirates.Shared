using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IDirectionable : ICommandable<DirectionCommand>
    {
        void SetDirection(fix2 direction);
        void ICommandable<DirectionCommand>.Execute(DirectionCommand cmd) => SetDirection(cmd.Direction);
    }
}
