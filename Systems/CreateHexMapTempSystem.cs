using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;

namespace DVG.SkyPirates.Shared.Systems
{
    [Obsolete]
    public class CreateHexMapTempSystem : ITickableExecutor
    {
        private bool _created = false;
        private readonly IPathFactory<HexMap> _hexMapLoader;
        private readonly World _world;

        public CreateHexMapTempSystem(IPathFactory<HexMap> hexMapLoader, World world)
        {
            _hexMapLoader = hexMapLoader;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            if (_created)
                return;

            var hexMap = _hexMapLoader.Create("Configs/Maps/Map");
            _world.Create(hexMap);
            _created = true;
        }
    }
}
