using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Json;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public static class CommandDataSerializer
    {
        public static CommandsData Serialize(ITimelineService timeline, int tick)
        {
            var commands = timeline.GetCommandsAfter(tick);

            Dictionary<string, List<string>> _commands = new();
            foreach (var tickCollection in commands)
            {
                var action = new CollectCommandsAction(tickCollection, _commands);
                CommandIds.ForEachData(ref action);
            }
            return new CommandsData(_commands);
        }

        public static void Deserialize(ITimelineService timelineService, CommandsData commandsData)
        {
            var action = new ApplyCommandsAction(commandsData.Commands, timelineService);
            CommandIds.ForEachData(ref action);
        }

        private readonly struct CollectCommandsAction : IGenericAction<ICommandData>
        {
            private readonly CommandCollection _commandCollection;
            private readonly Dictionary<string, List<string>> _commands;

            public CollectCommandsAction(CommandCollection commandCollection, Dictionary<string, List<string>> commands)
            {
                _commandCollection = commandCollection;
                _commands = commands;
            }

            public void Invoke<T>() where T : ICommandData
            {
                var collection = _commandCollection.GetCollection<T>();
                if (collection == null)
                    return;

                string key = typeof(T).Name;
                if (_commands.TryGetValue(key, out var list))
                    _commands[key] = list = new();

                foreach (var item in collection)
                    list.Add(Serialization.Serialize(item));
            }
        }
        private readonly struct ApplyCommandsAction : IGenericAction<ICommandData>
        {
            private readonly IReadOnlyDictionary<string, List<string>> _commands;
            private readonly ITimelineService _timeline;

            public ApplyCommandsAction(IReadOnlyDictionary<string, List<string>> commands, ITimelineService timeline)
            {
                _commands = commands;
                _timeline = timeline;
            }

            public void Invoke<T>() where T : ICommandData
            {
                if (!_commands.TryGetValue(typeof(T).Name, out var commands))
                    return;

                foreach (var item in commands)
                {
                    var cmd = Serialization.Deserialize<Command<T>>(item);
                    _timeline.AddCommand(cmd);
                }
            }
        }
    }
}
