using Arch.Core;

namespace DVG.SkyPirates.Shared.Components.Special
{
    public static class SingletonComponent
    {
        public static ref T GetSingleton<T>(this World world)
        {
            if (!world.TryGetArchetype(Component<T>.Signature, out var archetype))
            {
                var entity = world.Create<T>();
                archetype = world.GetArchetype(entity);
            }
            return ref archetype.GetChunk(0).GetFirst<T>();
        }
    }
}
