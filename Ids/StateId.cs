using DVG.Core.Ids.Attributes;

namespace DVG.SkyPirates.Shared.Ids
{
    [Id]
    [Flag]
    public partial struct StateId
    {
        public override int GetHashCode()
        {
            int hash = 0;

            if (!IsNone)
                for (int i = 0; i < Value.Length; i++)
                    hash += Value[i];

            return hash;
        }
    }
}
