namespace DVG.SkyPirates.Shared.Components.Special
{
    public struct History<T> where T : struct
    {
        public int CurrentTick;
        public readonly T?[] Data;

        public History(int length)
        {
            Data = new T?[length];
            CurrentTick = 0;
        }

        public static History<T> Create() => new History<T>(Constants.HistoryTicksLimit);
    }
}
