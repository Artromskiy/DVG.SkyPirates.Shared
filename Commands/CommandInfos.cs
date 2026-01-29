using DVG.Core;

namespace DVG.SkyPirates.Shared.Commands
{
    public static class CommandInfos
    {
        public static bool ClientPredicted<T>() where T : ICommandData
        {
            return typeof(T) == typeof(JoystickCommand);
        }
    }
}
