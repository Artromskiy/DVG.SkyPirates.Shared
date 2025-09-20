namespace DVG.SkyPirates.Shared.Components.Special
{
    public readonly struct History<T> where T : struct
    {
        public readonly T?[] Data;

        public History(int length)
        {
            Data = new T?[length];
        }

        public static History<T> Create() => new History<T>(Constants.HistoryTicksLimit);
    }
}
