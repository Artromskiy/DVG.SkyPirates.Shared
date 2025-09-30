using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    public class CommandsData
    {
        [DataMember]//               type,        cmd
        private readonly Dictionary<string, List<string>> _commands;
        public IReadOnlyDictionary<string, List<string>> Commands => _commands;

        public CommandsData(Dictionary<string, List<string>> commands)
        {
            _commands = commands;
        }
    }
}
