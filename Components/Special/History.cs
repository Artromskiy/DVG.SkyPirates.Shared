
namespace DVG.SkyPirates.Shared.Components.Special
{
    internal readonly struct History<T> where T : struct
    {
        //private readonly Dictionary<int, T?> Data;
        private readonly T?[] _data;

        public History(int length)
        {
            //Data = new Dictionary<int, T?>();
            _data = new T?[length];
        }

        public ref T? this[int tick] => ref _data[Constants.WrapTick(tick)];

        public bool AllHasValues()
        {
            foreach (var item in _data)
                if (!item.HasValue)
                    return false;
            return true;
        }

        public void SetValue(T? value, int tick)
        {
            tick = Constants.WrapTick(tick);
            _data[tick] = value;
        }

        public static History<T> Create() => new History<T>(Constants.HistoryTicks);
    }
}
