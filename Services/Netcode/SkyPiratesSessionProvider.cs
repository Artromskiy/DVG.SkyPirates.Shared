using Delta.Netcode;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Shared.Services.Netcode
{
    public sealed class SkyPiratesSessionProvider
    {
        private readonly SkyPiratesSessionSetup _setup;
        private readonly SessionMode _mode;
        private readonly ISessionTransport _transport;
        private readonly ICommandReciever _receiver;
        private bool _started;

        public SkyPiratesSessionProvider(
            SkyPiratesSessionSetup setup,
            SessionMode mode,
            ISessionTransport transport,
            ICommandReciever receiver)
        {
            _setup = setup;
            _mode = mode;
            _transport = transport;
            _receiver = receiver;
        }

        public event Action Ready;

        public SessionHost Session { get; private set; }

        public bool IsReady { get; private set; }

        public long CurrentStep => Session == null ? -1 : Session.CurrentStep;

        public void Start(AuthorId authorId)
        {
            if (_started)
            {
                throw new InvalidOperationException("The SkyPirates session has already started.");
            }

            _started = true;
            Session = _setup.Create(_mode, authorId, _transport);
            _transport.Received += OnMessage;

            if (_mode == SessionMode.Server)
            {
                Server = new SessionServer(_transport);
                Server.Add(Session);
                IsReady = true;
                Ready?.Invoke();
            }
            else if (_mode == SessionMode.Local)
            {
                IsReady = true;
                Ready?.Invoke();
            }
            else
            {
                Session.AttachConnection(0);
            }
        }

        public ISessionServer Server { get; private set; }

        public void Bind(ulong connectionId, AuthorId authorId)
        {
            if (_mode != SessionMode.Server || Server == null || Session == null)
            {
                throw new InvalidOperationException("Only a started server session can bind a client.");
            }

            Server.Bind(connectionId, Session, authorId);
            SendSnapshot(connectionId);
        }

        public CommandKey Send<T>(in T payload, long simulationStep)
        {
            if (!IsReady || Session == null)
            {
                throw new InvalidOperationException("The session is not ready to send commands.");
            }

            return Session.Send(payload, simulationStep);
        }

        public void Tick(long simulationStep)
        {
            if (IsReady)
            {
                Session.Tick(simulationStep);
            }
        }

        private void OnMessage(ulong connectionId, byte[] message)
        {
            if (Session == null)
            {
                return;
            }

            if (_mode == SessionMode.Server)
            {
                Server.Receive(connectionId, message);
                return;
            }

            if (_mode != SessionMode.Client)
            {
                return;
            }

            if (CommandProtocol.TryReadSnapshot(message, out SessionSnapshot snapshot))
            {
                Session.Restore(snapshot);
                IsReady = true;
                Ready?.Invoke();
                return;
            }

            if (CommandProtocol.TryReadOutcome(message, out CommandOutcome outcome, out ReadOnlySpan<byte> finalPayload))
            {
                Session.ApplyOutcome(outcome, finalPayload);
                if (outcome.Result == CommandResult.Accepted)
                {
                    var notify = new NotifyCommand(_receiver, finalPayload.ToArray(), outcome.Header);
                    Session.VisitCommandType(outcome.Header.TypeId, ref notify);
                }
            }
        }

        private void SendSnapshot(ulong connectionId)
        {
            SessionSnapshot snapshot = Session.CaptureSnapshot();
            var output = new ArrayBufferWriter<byte>();
            CommandProtocol.WriteSnapshot(snapshot, output);
            _transport.Send(connectionId, output.WrittenSpan);

            foreach (JournalRecord record in Session.ReadAcceptedAfter(snapshot.Cursor))
            {
                var outcome = new CommandOutcome(record.Result, record.Header);
                byte[] frame = CommandProtocol.EncodeOutcome(outcome, record.FinalPayload.Span);
                _transport.Send(connectionId, frame);
            }
        }

        private struct NotifyCommand : ICommandVisitor
        {
            private readonly ICommandReciever _receiver;
            private readonly byte[] _payload;
            private readonly CommandHeader _header;

            public NotifyCommand(ICommandReciever receiver, byte[] payload, CommandHeader header)
            {
                _receiver = receiver;
                _payload = payload;
                _header = header;
            }

            public void Visit<T>()
            {
                var payload = new SkyPiratesCommandPayloadHandler().Read<T>(_payload);
                _receiver.InvokeCommand(new Command<T>(_header, payload));
            }
        }
    }

    public sealed class SkyPiratesSessionTickLoop
    {
        private readonly SkyPiratesSessionProvider _session;
        private readonly ITickableService<IPreTickable> _preTickables;
        private readonly ITickableService<IPostTickable> _postTickables;

        public SkyPiratesSessionTickLoop(
            SkyPiratesSessionProvider session,
            ITickableService<IPreTickable> preTickables,
            ITickableService<IPostTickable> postTickables)
        {
            _session = session;
            _preTickables = preTickables;
            _postTickables = postTickables;
        }

        public bool Tick(long targetStep)
        {
            if (!_session.IsReady || _session.CurrentStep == targetStep)
            {
                return false;
            }

            int tick = checked((int)targetStep);
            _preTickables.Tick(tick);
            _session.Tick(targetStep);
            _postTickables.Tick(tick);
            return true;
        }
    }
}
