
namespace DVG.SkyPirates.Shared.Commands
{
    public readonly struct UnregisterSquadUnit
    {
        public readonly int unitId;

        public UnregisterSquadUnit(int unitId)
        {
            this.unitId = unitId;
        }
    }
}
