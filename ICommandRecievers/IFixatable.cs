using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandRecievers
{
    public interface IFixatable : ICommandable<Fixation>
    {
        void SetFixation(bool fixation);
        void ICommandable<Fixation>.Recieve(Fixation cmd) => SetFixation(cmd.fixation);
    }
}
