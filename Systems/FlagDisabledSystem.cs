using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class FlagDisabledSystem : IDeltaTickableExecutor
    {
        private readonly World _world;
        private readonly List<Entity> _toDisable = new();
        private readonly List<Entity> _toEnable = new();
        private readonly List<fix4> _activeQuads = new();

        private readonly QueryDescription _activityRanges = new QueryDescription().WithAll<ActivityRange, Position>().NotDisposing();

        private readonly QueryDescription _enabled = new QueryDescription().WithAll<Position>().WithNone<Disabled>().NotDisposing();
        private readonly QueryDescription _disabled = new QueryDescription().WithAll<Position, Disabled>().NotDisposing();

        public FlagDisabledSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _toDisable.Clear();
            _toEnable.Clear();
            _activeQuads.Clear();

            _world.Query(in _activityRanges, (ref ActivityRange range, ref Position position) =>
            {
                var pos = position.Value.xz;
                fix4 minMax = default;
                minMax.xy = pos - new fix2(range.Value);
                minMax.zw = pos + new fix2(range.Value);
                _activeQuads.Add(minMax);
            });

            var selectToDisable = new SelectToDisableQuery(_activeQuads, _toDisable);
            var selectToEnable = new SelectToEnableQuery(_activeQuads, _toEnable);
            _world.InlineEntityQuery<SelectToDisableQuery, Position>(in _enabled, ref selectToDisable);
            _world.InlineEntityQuery<SelectToEnableQuery, Position>(in _disabled, ref selectToEnable);

            foreach (var item in _toEnable)
                _world.Remove<Disabled>(item);
            foreach (var item in _toDisable)
                _world.Add<Disabled>(item);
        }

        private readonly struct SelectToDisableQuery : IForEachWithEntity<Position>
        {
            private readonly List<fix4> _minMaxs;
            private readonly List<Entity> _selection;

            public SelectToDisableQuery(List<fix4> minMaxs, List<Entity> mark)
            {
                _minMaxs = minMaxs;
                _selection = mark;
            }

            public readonly void Update(Entity entity, ref Position position)
            {
                var xz = position.Value.xz;

                bool anyInside = false;
                for (int i = 0; i < _minMaxs.Count; i++)
                    anyInside |= Inside(xz, _minMaxs[i]);

                if (!anyInside)
                    _selection.Add(entity);
            }
        }

        private readonly struct SelectToEnableQuery : IForEachWithEntity<Position>
        {
            private readonly List<fix4> _minMaxs;
            private readonly List<Entity> _selection;

            public SelectToEnableQuery(List<fix4> minMaxs, List<Entity> unmark)
            {
                _minMaxs = minMaxs;
                _selection = unmark;
            }

            public readonly void Update(Entity entity, ref Position position)
            {
                var xz = position.Value.xz;
                for (int i = 0; i < _minMaxs.Count; i++)
                {
                    if (Inside(xz, _minMaxs[i]))
                    {
                        _selection.Add(entity);
                        return;
                    }
                }
            }
        }

        private static bool Inside(fix2 point, fix4 minMax)
        {
            return point.x >= minMax.x && point.y >= minMax.y && point.x <= minMax.z && point.y <= minMax.w;
        }
    }
}
