using DVG.Core;
using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands.Lifecycle
{
    [Command]
    public readonly struct SpawnSquad : ICommandData
    {
        public int CommandId => throw new System.NotImplementedException();
    }
}
