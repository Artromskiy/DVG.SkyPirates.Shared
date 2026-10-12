using Delta.ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class ComponentIdList : IReadOnlyList<ComponentIdEntry>
    {
        private readonly List<ComponentIdEntry> _items = new();
        private ComponentSchemaCatalog _catalog = null!;
        public int Count => _items.Count;
        public ComponentIdEntry this[int index] => _items[index];
        public IEnumerator<ComponentIdEntry> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal void Add(ComponentIdEntry entry) => _items.Add(entry);
        internal void Bind(ComponentSchemaCatalog catalog) => _catalog = catalog;
        internal void Visit(ComponentIdEntry entry, IStructVisitor visitor) => _catalog.Visit(entry.ComponentId, visitor);
        public ComponentId[] GetIds()
        {
            var ids = new ComponentId[_items.Count];
            for (int i = 0; i < ids.Length; i++) ids[i] = _items[i].ComponentId;
            return ids;
        }
    }

    public sealed class ComponentIdEntry
    {
        public string Name { get; }
        public ComponentId ComponentId { get; internal set; } = ComponentId.Invalid;
        public ComponentIdEntry(string name) => Name = name;
    }

}
