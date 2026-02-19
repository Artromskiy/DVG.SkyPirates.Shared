using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DVG.SkyPirates.Shared.Systems
{
    // another heat death of universe system
    public class SquadGoodsDistributionSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly Dictionary<int, Dictionary<GoodsId, int>> _goodsPerSquad = new();
        private readonly Dictionary<int, SortedList<GoodsId, int>> _goodsPerUnit = new();
        private readonly Dictionary<int, List<SyncId>> _unitsPerSquad = new();

        private readonly QueryDescription _unitsDesc = new QueryDescription().
            WithAll<SquadMember, SyncId, GoodsDrop>().NotDisposing().NotDisabled();

        public SquadGoodsDistributionSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var item in _unitsPerSquad)
                item.Value.Clear();
            foreach (var item in _goodsPerSquad)
                item.Value.Clear();
            foreach (var item in _goodsPerUnit)
                item.Value.Clear();

            var squadMembersQuery = new SquadMembersCollectQuery(_goodsPerSquad, _unitsPerSquad);
            _world.InlineQuery<SquadMembersCollectQuery, SquadMember, GoodsDrop, SyncId>(_unitsDesc, ref squadMembersQuery);

            foreach (var item in _unitsPerSquad)
                item.Value.Sort((u1, u2) => u1.Value.CompareTo(u2.Value));

            foreach (var (squadId, units) in _unitsPerSquad)
            {
                if (!_goodsPerSquad.TryGetValue(squadId, out var squadGoods))
                    continue;

                int membersCount = units.Count;
                if (membersCount == 0)
                    continue;

                foreach (var goodsId in squadGoods.Keys.OrderBy(k => k))
                {
                    int total = squadGoods[goodsId];

                    int baseAmount = total / membersCount;
                    int remainder = total % membersCount;

                    for (int i = 0; i < membersCount; i++)
                    {
                        int amount = baseAmount + (i < remainder ? 1 : 0);
                        if (amount <= 0)
                            continue;

                        var syncId = units[i];

                        if (!_goodsPerUnit.TryGetValue(syncId, out var unitGoods))
                            _goodsPerUnit[syncId] = unitGoods = new SortedList<GoodsId, int>();

                        if (!unitGoods.ContainsKey(goodsId))
                            unitGoods[goodsId] = 0;
                        unitGoods[goodsId] += amount;
                    }
                }
            }

            var distributeQuery = new SquadMembersDistributeQuery(_goodsPerUnit);
            _world.InlineQuery<SquadMembersDistributeQuery, SyncId, GoodsDrop>(_unitsDesc, ref distributeQuery);
        }

        private readonly struct SquadMembersDistributeQuery : IForEach<SyncId, GoodsDrop>
        {
            private readonly Dictionary<int, SortedList<GoodsId, int>> _goodsPerUnit;

            public SquadMembersDistributeQuery(Dictionary<int, SortedList<GoodsId, int>> goodsPerUnit)
            {
                _goodsPerUnit = goodsPerUnit;
            }

            public void Update(ref SyncId syncId, ref GoodsDrop drop)
            {
                if (!_goodsPerUnit.TryGetValue(syncId, out var distributedDrop) ||
                    distributedDrop.Count == 0)
                {
                    drop = new() { Values = ImmutableSortedDictionary<GoodsId, int>.Empty };
                    return;
                }

                // uses object comparer by default!!!
                if (drop.Values?.SequenceEqual(distributedDrop, KeyValuePairComparer<GoodsId, int>.Default) ?? false)
                    return;

                drop = new() { Values = distributedDrop.ToImmutableSortedDictionary() };
            }
        }

        private readonly struct SquadMembersCollectQuery : IForEach<SquadMember, GoodsDrop, SyncId>
        {
            private readonly Dictionary<int, Dictionary<GoodsId, int>> _goodsPerSquad;
            private readonly Dictionary<int, List<SyncId>> _unitsPerSquad;

            public SquadMembersCollectQuery(Dictionary<int, Dictionary<GoodsId, int>> goodsPerSquad, Dictionary<int, List<SyncId>> unitsPerSquad)
            {
                _goodsPerSquad = goodsPerSquad;
                _unitsPerSquad = unitsPerSquad;
            }

            public void Update(ref SquadMember member, ref GoodsDrop drop, ref SyncId syncId)
            {
                if (drop.Values != null)
                {
                    if (!_goodsPerSquad.TryGetValue(member.SquadId, out var squadGoods))
                        _goodsPerSquad[member.SquadId] = squadGoods = new Dictionary<GoodsId, int>();

                    foreach (var item in drop.Values)
                    {
                        if (item.Value <= 0)
                            continue;

                        if (!squadGoods.TryAdd(item.Key, item.Value))
                            squadGoods[item.Key] += item.Value;
                    }
                }

                if (!_unitsPerSquad.TryGetValue(member.SquadId, out var list))
                    _unitsPerSquad[member.SquadId] = list = new();
                list.Add(syncId);
            }
        }
    }
}