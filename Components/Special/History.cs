
namespace DVG.SkyPirates.Shared.Components.Special
{
    internal readonly struct History<T> where T : struct
    {
        private readonly T?[] _data;
        public ref T? this[int tick] => ref _data[Constants.WrapTick(tick)];

        public History(int length) => _data = new T?[length];

        public static History<T> Create() => new(Constants.HistoryTicks);
    }
}