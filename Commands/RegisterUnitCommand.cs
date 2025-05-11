using System;

namespace DVG.SkyPirates.Shared.Commands
{
    [Obsolete]
    public struct RegisterUnitCommand
    {
        public int id;

        public RegisterUnitCommand(int id)
        {
            this.id = id;
        }
    }
}
