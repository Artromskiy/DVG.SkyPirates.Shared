using Arch.Core;
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

            var squad = _world.FirstOrDefaultEntity<Squad>();
            var pos = _world.Get<Position>(squad).Value.xz;
            fix4 minMax = default;
            minMax.xy = pos - new fix2(10);
            minMax.zw = pos + new fix2(10);

            var selectToDisable = new SelectDisableQuery(minMax, _toDisable);
            var selectToEnable = new SelectEnableQuery(minMax, _toEnable);
            _world.InlineEntityQuery<SelectDisableQuery, Position>(in _disabled, ref selectToDisable);
            _world.InlineEntityQuery<SelectEnableQuery, Position>(in _enabled, ref selectToEnable);

            foreach (var item in _toEnable)
                _world.Remove<Disabled>(item);
            foreach (var item in _toDisable)
                _world.Add<Disabled>(item);
        }

        private readonly struct SelectDisableQuery : IForEachWithEntity<Position>
        {
            private readonly fix4 _minMax;
            private readonly List<Entity> _mark;

            public SelectDisableQuery(fix4 minMax, List<Entity> mark)
            {
                _minMax = minMax;
                _mark = mark;
            }

            public readonly void Update(Entity entity, ref Position position)
            {
                var xz = position.Value.xz;
                if (xz.x < _minMax.x || xz.y < _minMax.y || xz.x > _minMax.z || xz.y > _minMax.y)
                    _mark.Add(entity);
            }
        }

        private readonly struct SelectEnableQuery : IForEachWithEntity<Position>
        {
            private readonly fix4 _minMax;
            private readonly List<Entity> _unmark;

            public SelectEnableQuery(fix4 minMax, List<Entity> unmark)
            {
                _minMax = minMax;
                _unmark = unmark;
            }

            public readonly void Update(Entity entity, ref Position position)
            {
                var xz = position.Value.xz;
                if (xz.x >= _minMax.x && xz.y >= _minMax.y && xz.x <= _minMax.z && xz.y <= _minMax.y)
                    _unmark.Add(entity);
            }
        }
    }
}
