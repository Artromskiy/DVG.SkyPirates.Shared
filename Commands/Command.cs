using DVG.Core;
using System;

namespace DVG.SkyPirates.Shared.Commands
{
    public readonly struct Command<C> : ICommand, IComparable<Command<C>> where C : unmanaged
    {
        public readonly int callerId;
        public readonly TimeSpan timeStamp;
        public readonly C data;

        public Command(int callerId, TimeSpan timeStamp, C data)
        {
            this.callerId = callerId;
            this.timeStamp = timeStamp;
            this.data = data;
        }

        public readonly int CommandId => CommandIds.GetId<C>();
        readonly int ICommand.CallerId => callerId;
        readonly TimeSpan ICommand.TimeStamp => timeStamp;
        readonly int IComparable<Command<C>>.CompareTo(Command<C> other) => timeStamp.CompareTo(other);
        readonly int IComparable<ICommand>.CompareTo(ICommand other) => timeStamp.CompareTo(other.TimeStamp);
    }
}
