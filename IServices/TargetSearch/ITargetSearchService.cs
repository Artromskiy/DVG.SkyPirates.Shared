namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITargetSearchService
    {
        ITarget FindTarget(fix3 position, fix distance);
        ITarget[] FindTargets(fix3 position, fix distance);
        void UpdateTargetsSearch();
    }
}
