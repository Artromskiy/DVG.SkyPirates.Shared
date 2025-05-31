using DVG.Core;
using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands.Lifecycle
{
    [Command]
    public readonly struct DespawnSquad : ICommandData
    {
        public int CommandId => throw new System.NotImplementedException();
    }
}
