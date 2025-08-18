using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IFixationable : ICommandable<FixationCommand>
    {
        void SetFixation(bool fixation);
        void ICommandable<FixationCommand>.Execute(FixationCommand cmd) => SetFixation(cmd.Fixation);
    }
}
