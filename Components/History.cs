using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components
{
    public struct History<T>
    {
        public Dictionary<int, T> history;

        public static History<T> Create() => new History<T>()
        {
            history = new Dictionary<int, T>()
        };
    }
}
