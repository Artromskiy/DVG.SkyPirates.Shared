
namespace DVG.SkyPirates.Shared.Components.Special
{
    internal readonly struct History<T> where T : struct
    {
        //private readonly Dictionary<int, T?> Data;
        private readonly T?[] Data;

        public History(int length)
        {
            //Data = new Dictionary<int, T?>();
            Data = new T?[length];
        }

        public bool HasValue(int tick)
        {
            tick = Constants.WrapTick(tick);
            return Data[tick].HasValue;
            //return Data.ContainsKey(tick) && Data[tick].HasValue;
        }

        public T GetValue(int tick)
        {
            tick = Constants.WrapTick(tick);
            return Data[tick].Value;
            //return Data[tick].Value;
        }

        public void SetValue(T? value, int tick)
        {
            tick = Constants.WrapTick(tick);
            Data[tick] = value;
        }

        public static History<T> Create() => new History<T>(Constants.HistoryTicks);
    }
}
