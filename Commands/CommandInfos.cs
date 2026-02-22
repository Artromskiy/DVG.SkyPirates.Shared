namespace DVG.SkyPirates.Shared.Commands
{
    public static class CommandInfos
    {
        public static bool ClientPredicted<T>()
        {
            return false;
            return typeof(T) == typeof(JoystickCommand);
        }
    }
}
