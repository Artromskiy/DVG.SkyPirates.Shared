﻿using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.HistorySystems
{
    public sealed class LogHashSumSystem : ITickableExecutor
    {
        private readonly Descriptions _descriptions = new Descriptions();
        private readonly World _world;

        public LogHashSumSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var action = new LogHashAction(_descriptions, _world);
            HistoryIds.ForEachData(ref action);
            Console.WriteLine($"Tick: {tick}, Hash: {action.Hash}");
        }

        private struct LogHashAction : IStructGenericAction
        {
            private readonly Descriptions _descriptions;
            private readonly World _world;
            public int Hash;

            public LogHashAction(Descriptions descriptions, World world)
            {
                _descriptions = descriptions;
                _world = world;
                Hash = 0;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new HasSumQuery<T>();
                var desc = _descriptions.GetDescription<T>().Desc;
                _world.InlineQuery<HasSumQuery<T>, T>(desc, ref query);
                Hash += query.Hash;
            }
        }

        private struct HasSumQuery<T> : IForEach<T>
            where T : struct
        {
            public int Hash;

            public void Update(ref T component)
            {
                Hash += component.GetHashCode();
            }

        }

        private interface IDescription { }
        private sealed class Descriptions
        {
            private readonly Dictionary<Type, IDescription> _descriptions = new Dictionary<Type, IDescription>();
            public Description<T> GetDescription<T>() where T : struct
            {
                var type = typeof(T);
                if (!_descriptions.TryGetValue(type, out var description))
                    _descriptions[type] = description = new Description<T>();
                return (Description<T>)description;
            }
        }

        private sealed class Description<T> : IDescription where T : struct
        {
            public readonly QueryDescription Desc = new QueryDescription().WithAll<T>();
        }
    }
}
