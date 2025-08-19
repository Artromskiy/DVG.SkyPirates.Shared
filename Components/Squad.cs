using Arch.Core;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components
{
    public struct Squad
    {
        public readonly List<Entity> _units;
        public readonly List<int> _orders;
        public fix2[] _rotatedPoints;
    }
}
