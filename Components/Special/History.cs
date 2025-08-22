using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components.Special
{
    public struct History<T> where T : struct
    {
        public Dictionary<int, T?> history;

        public static History<T> Create() => new History<T>()
        {
            history = new Dictionary<int, T?>()
        };
    }
}
