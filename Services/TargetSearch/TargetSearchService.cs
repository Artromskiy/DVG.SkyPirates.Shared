#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TargetSearch;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVG.SkyPirates.Shared.Services.TargetSearch
{
    public class TargetSearchService : ITargetSearchService
    {
        private const int SquareSize = 35;
        private readonly Dictionary<int2, List<ITarget>> _targets = new Dictionary<int2, List<ITarget>>();

        private IEntitiesService _entitiesService;

        public ITarget FindTarget(real3 position, real distance)
        {
            throw new NotImplementedException();
        }

        public ITarget[] FindTargets(real3 position, real distance)
        {
            throw new NotImplementedException();
        }

        public void UpdateTargetsSearch()
        {
            foreach (var item in _targets)
                item.Value.Clear();

            foreach (var entityId in _entitiesService.GetEntityIds())
            {
                if (!_entitiesService.TryGetEntity<ITarget>(entityId, out var target))
                    continue;

                var intPos = GetQuantizedSquare(target.Position);
                if (!_targets.TryGetValue(intPos, out var targets))
                    _targets[intPos] = targets = new List<ITarget>();
                targets.Add(target);
            }
        }

        private int2 GetQuantizedSquare(real3 position)
        {
            var pos = position / SquareSize;
            return new int2((int)pos.x, (int)pos.z);
        }
    }
}
