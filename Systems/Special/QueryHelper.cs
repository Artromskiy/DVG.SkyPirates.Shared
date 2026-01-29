using Arch.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Tools;

namespace DVG.SkyPirates.Shared.Systems.Linq
{
    public static class QueryHelper
    {
        private class Description<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T>();
        }

        private static readonly QueryDescription NotUsedDesc = new QueryDescription().WithNone<Free>();
        private static readonly GenericCollection _desc = new();


        public static T FirstOrDefault<T>(this World world) where T : struct
        {
            var query = new FirstOrDefaultQuery<T>();
            var desc = _desc.Get<Description<T>>().Desc;
            world.InlineQuery<FirstOrDefaultQuery<T>, T>(in desc, ref query);
            return query.Value;
        }

        public static int MaxEntityId(this World world)
        {
            var query = new MaxEntityIdQuery();
            world.InlineQuery(in NotUsedDesc, ref query);
            return query.Value;
        }

        private struct MaxEntityIdQuery : IForEach
        {
            public int Value;

            public void Update(Entity entity)
            {
                Value = Maths.Max(Value, entity.Id);
            }
        }

        private struct FirstOrDefaultQuery<T> : IForEach<T>
        {
            public T Value;
            private bool _valueSet;
            public void Update(ref T t)
            {
                if (_valueSet)
                    return;

                _valueSet = true;
                Value = t;
            }
        }
    }
}
