using System;
using System.Collections;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public class GenericCollection
    {
        public Dictionary<Type, IList> _lists = new Dictionary<Type, IList>();

        public void Add<T>(T value)
        {
            var key = typeof(T);

            if (!_lists.TryGetValue(key, out var list))
                _lists.Add(key, list = new List<T>());

            if (list is List<T> genericList)
                genericList.Add(value);
        }

        public bool Remove<T>(Predicate<T> match)
        {
            var key = typeof(T);
            if (!_lists.TryGetValue(key, out var list))
                return false;

            if (!(list is List<T> genericList))
                return false;

            var index = genericList.FindIndex(match);
            if (index == -1)
                return false;

            genericList.RemoveAt(index);
            return true;
        }

        public void Clear() => _lists.Clear();

        public List<T> GetCollection<T>()
        {
            var key = typeof(T);
            if (!_lists.TryGetValue(key, out var list))
                _lists.Add(key, list = new List<T>());

            if (!(list is List<T> genericList))
                return null!;

            return genericList;
        }
    }
}
