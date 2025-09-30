using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public class CommandsData
    {
        [DataMember(Order = 0)]
        private readonly Dictionary<string, List<string>> _commands;
        [IgnoreDataMember]
        public IReadOnlyDictionary<string, List<string>> Commands => _commands;

        public CommandsData(Dictionary<string, List<string>> commands)
        {
            _commands = commands;
        }
    }
}
