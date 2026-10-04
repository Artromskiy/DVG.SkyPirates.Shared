using System.Threading;
using Delta.Netcode;

namespace DVG.SkyPirates.Shared.Commands
{
    public static class SkyPiratesCommand
    {
        private static long _nextSequence;

        public static Command<T> Create<T>(T payload) => Create(-1, -1, payload);

        public static Command<T> Create<T>(int clientId, int tick, T payload)
        {
            var registration = GeneratedCommands.GetRegistration<T>();
            var key = new CommandKey(
                new SessionId(0),
                new AuthorId(unchecked((uint)clientId)),
                unchecked((ulong)Interlocked.Increment(ref _nextSequence)));
            var header = new CommandHeader(key, registration.Id, tick, 0);
            return new Command<T>(header, payload);
        }

        public static Command<T> WithClientId<T>(Command<T> command, int clientId)
        {
            var key = command.Header.Key;
            var updatedKey = new CommandKey(key.SessionId, new AuthorId(unchecked((uint)clientId)), key.Sequence);
            var header = new CommandHeader(updatedKey, command.Header.TypeId, command.Header.Step, command.Header.Order);
            return new Command<T>(header, command.Payload);
        }

        public static Command<T> WithTick<T>(Command<T> command, int tick)
        {
            var header = new CommandHeader(command.Header.Key, command.Header.TypeId, tick, command.Header.Order);
            return new Command<T>(header, command.Payload);
        }

        public static int GetClientId<T>(Command<T> command) => unchecked((int)command.Header.Key.AuthorId.Value);

        public static int GetTick<T>(Command<T> command) => checked((int)command.Header.Step);
    }
}
