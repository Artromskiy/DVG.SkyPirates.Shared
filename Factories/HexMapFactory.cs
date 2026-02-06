using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class HexMapFactory : IHexMapFactory
    {
        private readonly ICommandEntityFactory _commandEntityFactory;
        private readonly World _world;

        public HexMapFactory(World world, ICommandEntityFactory commandEntityFactory)
        {
            _world = world;
            _commandEntityFactory = commandEntityFactory;
        }

        public Entity Create(int parameters)
        {
            var entity = _commandEntityFactory.Create(parameters);
            _world.AddOrGet<HexMap>(entity).Data = new();
            return entity;
        }
    }
}
