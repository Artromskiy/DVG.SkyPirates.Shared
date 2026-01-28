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
        public readonly Dictionary<string, List<string>> Commands;

        public CommandsData(Dictionary<string, List<string>> commands)
        {
            Commands = commands;
        }

        public static CommandsData Create() => new(new());
    }
}
