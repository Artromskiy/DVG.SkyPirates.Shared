using DVG.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class PackedCirclesFactory : IPackedCirclesFactory
    {
        private const string PathFormat = "Configs/PackedCircles/PackedCirclesModel{0}";
        private readonly Dictionary<int, PackedCirclesData> _circlesConfigCache = new();
        private readonly IPathFactory<PackedCirclesData> _circlesFactory;

        public PackedCirclesFactory(IPathFactory<PackedCirclesData> circlesFactory)
        {
            _circlesFactory = circlesFactory;
        }

        public PackedCirclesData Create(int parameters)
        {
            if (!_circlesConfigCache.TryGetValue(parameters, out var config))
                _circlesConfigCache[parameters] = config = _circlesFactory.Create(string.Format(PathFormat, parameters));

            return config;
        }
    }
}
