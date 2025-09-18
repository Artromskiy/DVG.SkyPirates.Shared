using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public partial class SquadConfig
    {
        [DataMember(Order = 0)]
        public UnitAndLevel[] cards;

        public SquadConfig(UnitAndLevel[] cards)
        {
            this.cards = cards;
        }
    }
}