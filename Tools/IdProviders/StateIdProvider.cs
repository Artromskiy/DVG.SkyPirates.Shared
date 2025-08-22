using DVG.Core.Tools;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools.IdProviders
{
    public class StateIdProvider : StringIdProvider<StateIdProvider, StateId>
    {
        public override IEnumerable<StateId> TypedIds => new StateId[]
        {
            new StateId("None"),
            new StateId("PreAttack"),
            new StateId("PostAttack"),
            new StateId("Reload"),
        };

    }
}