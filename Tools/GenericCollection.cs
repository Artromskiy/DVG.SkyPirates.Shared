using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools
{
    internal class GenericCollection
    {
        private readonly Dictionary<Type, object> _elements = new();
        public K Get<K>() where K : class, new()
        {
            var key = typeof(K);
            if (!_elements.TryGetValue(key, out var element))
            {
                _elements[key] = element = new K();
            }
            return (K)element!;
        }
    }
}
