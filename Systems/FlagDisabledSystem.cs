using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class FlagDisabledSystem : ITickableExecutor
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

            var selectToDisable = new SelectDisableQuery(_activeQuads, _toDisable);
            var selectToEnable = new SelectEnableQuery(_activeQuads, _toEnable);
            _world.InlineEntityQuery<SelectDisableQuery, Position>(in _disabled, ref selectToDisable);
            _world.InlineEntityQuery<SelectEnableQuery, Position>(in _enabled, ref selectToEnable);

            foreach (var item in _toEnable)
                _world.Remove<Disabled>(item);
            foreach (var item in _toDisable)
                _world.Add<Disabled>(item);
        }

        private readonly struct SelectDisableQuery : IForEachWithEntity<Position>
        {
            private readonly List<fix4> _minMaxs;
            private readonly List<Entity> _mark;

            public SelectDisableQuery(List<fix4> minMaxs, List<Entity> mark)
            {
                _minMaxs = minMaxs;
                _mark = mark;
            }

            public readonly void Update(Entity entity, ref Position position)
            {
                var xz = position.Value.xz;

                bool mark = true;
                for (int i = 0; i < _minMaxs.Count; i++)
                {
                    var minMax = _minMaxs[i];
                    if (xz.x < minMax.x || xz.y < minMax.y || xz.x > minMax.z || xz.y > minMax.y)
                        mark &= true;
                }
                if (mark)
                {
                    _mark.Add(entity);
                }
            }
        }

        private readonly struct SelectEnableQuery : IForEachWithEntity<Position>
        {
            private readonly List<fix4> _minMaxs;
            private readonly List<Entity> _unmark;

            public SelectEnableQuery(List<fix4> minMaxs, List<Entity> unmark)
            {
                _minMaxs = minMaxs;
                _unmark = unmark;
            }

            public readonly void Update(Entity entity, ref Position position)
            {
                var xz = position.Value.xz;
                for (int i = 0; i < _minMaxs.Count; i++)
                {
                    var minMax = _minMaxs[i];
                    if (xz.x >= minMax.x && xz.y >= minMax.y && xz.x <= minMax.z && xz.y <= minMax.y)
                    {
                        _unmark.Add(entity);
                        return;
                    }
                }
            }
        }
    }
}
