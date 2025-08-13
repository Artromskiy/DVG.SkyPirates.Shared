namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITarget
    {
        public fix3 Position { get; }
        public int Health { get; }
    }
}
