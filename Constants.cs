namespace DVG.SkyPirates.Shared
{
    public static class Constants
    {
        public const int TicksPerSecond = 10;
        public const int HistoryTicksLimit = 600;

        public static int WrapTick(int tick)
        {
            return tick & HistoryTicksLimit - 1;
        }
    }
}
