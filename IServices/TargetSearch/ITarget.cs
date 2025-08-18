namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITarget
    {
        public int TeamId { get; }
        public fix3 Position { get; }
        public fix Health { get; set; }
    }
}
