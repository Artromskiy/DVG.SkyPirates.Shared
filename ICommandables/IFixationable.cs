using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IFixationable : ICommandable<Fixation>
    {
        void SetFixation(bool fixation);
        void ICommandable<Fixation>.Execute(Fixation cmd) => SetFixation(cmd.fixation);
    }
}
