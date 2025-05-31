using DVG.Core;

namespace DVG.SkyPirates.Shared.Commands
{
    partial struct Fixation : ICommandData
    {
        public int CommandId => CommandIds.GetId<Fixation>();
    }
}
