using Delta.Netcode;

namespace DVG.SkyPirates.Shared.Services.Netcode
{
    public sealed class SkyPiratesSession
    {
        public SkyPiratesSession(SessionHost host, SkyPiratesSessionModel model, ICommandRegistry commands, ICommandJournal journal)
        {
            Host = host;
            Model = model;
            Commands = commands;
            Journal = journal;
        }

        public SessionHost Host { get; }
        public SkyPiratesSessionModel Model { get; }
        public ICommandRegistry Commands { get; }
        public ICommandJournal Journal { get; }
    }
}
