using DVG.Core;

namespace DVG.SkyPirates.Shared.Components
{
    public class HistoryComponents
    {
        public static void ForEachData<T>(T action) where T : IGenericAction
        {
            action.Invoke<Position>();
            action.Invoke<Rotation>();
            action.Invoke<Health>();
        }
    }
}
