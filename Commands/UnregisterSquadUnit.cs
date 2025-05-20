using DVG.Core.Commands.Attributes;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly struct UnregisterSquadUnit
    {
        public readonly int unitId;

        public UnregisterSquadUnit(int unitId)
        {
            this.unitId = unitId;
        }
    }
}
