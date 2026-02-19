namespace DVG.SkyPirates.Shared
{
    public static class Constants
    {
        public const int TicksPerSecond = 16;
        public const int HistoryTicks = 60_0; // 1 minute ?

        public static readonly fix TickTime = fix.One / TicksPerSecond;

        public static int WrapTick(int tick)
        {
            return tick & HistoryTicks - 1;
        }
    }
}
