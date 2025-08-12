using DVG.Json;
using System;

namespace DVG.SkyPirates.Shared.Configs
{
    [JsonAsset]
    [Serializable]
    public partial class SquadConfig
    {
        public UnitAndLevel[] cards;

        public SquadConfig(UnitAndLevel[] cards)
        {
            this.cards = cards;
        }
    }
}