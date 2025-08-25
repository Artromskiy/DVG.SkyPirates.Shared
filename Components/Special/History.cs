namespace DVG.SkyPirates.Shared.Components.Special
{
    public struct History<T> where T : struct
    {
        public T?[] history;

        public static History<T> Create() => new History<T>()
        {
            history = new T?[Constants.HistoryTicksLimit]
        };
    }
}
