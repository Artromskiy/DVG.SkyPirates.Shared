using Arch.Core;
using Arch.Core.Extensions.Dangerous;

namespace DVG.SkyPirates.Shared.Entities
{
    public static class EntityIds
    {
        public static Entity Get(int id)
        {
            var entity = DangerousEntityExtensions.CreateEntityStruct(id, 0, 1);
            return entity;
        }
    }
}
