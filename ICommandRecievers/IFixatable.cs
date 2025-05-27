using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.ICommandRecievers
{
    public interface IFixatable : ICommandReciever<Fixation>
    {
        void SetFixation(bool fixation);
        void ICommandReciever<Fixation>.Recieve(Fixation cmd) => SetFixation(cmd.fixation);
    }
}
