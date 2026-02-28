using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    public sealed class LogHashSumSystem : IDeltaTickableExecutor
    {
        private readonly Descriptions _descriptions = new Descriptions();
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly World _world;
        public LogHashSumSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var action = new LogHashAction(_descriptions, _stringBuilder, _world);
            _stringBuilder.Clear();
            HistoryComponentsRegistry.ForEachData(ref action);
            //Console.WriteLine($"Tick: {tick}, Hash: {action.Hash}" + Environment.NewLine + _stringBuilder.ToString());
        }

        public (int sum, string info) GetHashSum()
        {
            _stringBuilder.Clear();
            var action = new LogHashAction(_descriptions, _stringBuilder, _world);
            HistoryComponentsRegistry.ForEachData(ref action);
            return (action.Hash, _stringBuilder.ToString());
        }

        private struct LogHashAction : IStructGenericAction
        {
            private readonly Descriptions _descriptions;
            private readonly World _world;
            private readonly StringBuilder _stringBuilder;
            public int Hash;

            public LogHashAction(Descriptions descriptions, StringBuilder stringBuilder, World world)
            {
                _descriptions = descriptions;
                _stringBuilder = stringBuilder;
                _world = world;
                Hash = 0;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new HasSumQuery<T>();
                var desc = _descriptions.GetDescription<T>().Desc;
                _world.InlineQuery<HasSumQuery<T>, T>(desc, ref query);
                _stringBuilder.AppendLine($"Hash of {typeof(T).Name}: {query.Hash}");
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
