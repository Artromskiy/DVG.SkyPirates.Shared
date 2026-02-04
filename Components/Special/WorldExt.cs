using Arch.Core;

namespace DVG.SkyPirates.Shared.Components.Special
{
    public static class WorldExt
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

        public static void AddQuery<Has, Add>(this World world, ForEach<Has, Add> forEach)
        {
            var addDesc = new QueryDescription().WithAll<Has>().WithNone<Add>();
            world.Add<Add, Temp>(addDesc);
            var queryDesc = new QueryDescription().WithAll<Add, Temp>();
            world.Query(queryDesc, forEach);
            world.Remove<Temp>(queryDesc);
        }
    }
}
