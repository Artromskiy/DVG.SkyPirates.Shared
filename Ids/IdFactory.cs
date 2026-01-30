using System.Runtime.CompilerServices;

namespace DVG.SkyPirates.Shared.Ids
{
    public class IdFactory
    {
        public static T Create<T>(string value)
        {
            var internalId = new InternalId(value);
            return Unsafe.As<InternalId, T>(ref internalId);
        }
    }
}
