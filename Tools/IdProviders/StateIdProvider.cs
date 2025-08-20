using DVG.Core;
using DVG.Core.Tools;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools.IdProviders
{
    public class StateIdProvider : StringIdProvider<StateIdProvider, StateId>
    {
        public override IEnumerable<StateId> TypedIds => new StateId[]
        {
            new StateId("PreAttack"),
            new StateId("Impact"),
            new StateId("PostAttack"),
            new StateId("Reload"),
            new StateId("Move"),
        };

    }
}
namespace DVG.SkyPirates.Shared.Ids
{
    public partial struct StateId
    {
        public void GenericCall(IGenericAction action)
        {
            Flags.ForEachFlag(this, action);
        }

        public class Flags
        {
            public readonly struct PreAttack { }
            public readonly struct Impact { }
            public readonly struct PostAttack { }
            public readonly struct Reload { }
            public readonly struct Move { }

            public static void ForEachFlag(StateId stateId, IGenericAction genericAction)
            {
                _ = stateId.value switch
                {
                    nameof(PreAttack) => GenericCall<PreAttack>(genericAction),
                    nameof(Impact) => GenericCall<Impact>(genericAction),
                    nameof(PostAttack) => GenericCall<PostAttack>(genericAction),
                    nameof(Reload) => GenericCall<Reload>(genericAction),
                    nameof(Move) => GenericCall<Move>(genericAction),
                    _ => throw new NotImplementedException()
                };
            }

            private static int GenericCall<T>(IGenericAction action)
            {
                action.Invoke<T>();
                return 0;
            }
        }
    }
}