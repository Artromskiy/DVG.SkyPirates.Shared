using DVG.Core.History.Attributes;
using DVG.Core.Ids.Attributes;

namespace DVG.SkyPirates.Shared.Ids
{
    [Id]
    [History]
    public partial struct UnitId
    {
        /*
        public override readonly int GetHashCode()
        {
            int hash = 0;

            if (!IsNone)
                for (int i = 0; i < Value.Length; i++)
                    hash += Value[i];

            return hash;
        }
        */
    }
}
