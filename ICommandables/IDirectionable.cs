using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IDirectionable : ICommandable<Direction>
    {
        void SetDirection(fix2 direction);
        void ICommandable<Direction>.Execute(Direction cmd) => SetDirection(cmd.direction);
    }
}
