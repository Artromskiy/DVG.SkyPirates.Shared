using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;
using Riptide;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class MessageIO
    {
        private byte[] _tempBytes = Array.Empty<byte>();
        private readonly ArrayBufferWriter<byte> _buffer = new();
        private readonly ICommandSerializer _commandSerializer;
        private ushort _splitMessageId;

        // split  last + index + uid
        private const int SplitHeaderSize = 5; // bool + ushort + ushort + bool
        private int SplitSize => Message.MaxPayloadSize - SplitHeaderSize;

        private readonly Dictionary<(ushort client, ushort uid), SplitMessageStorage> _splitMessages = new();
        private readonly Queue<SplitMessageStorage> _queue = new();

        public MessageIO(ICommandSerializer commandSerializer)
        {
            _commandSerializer = commandSerializer;
        }

        public bool RecieveMessage<T>(Message message, int client, out Command<T> command)
        {
            bool splitted = message.GetBool();
            _buffer.Clear();

            if (!splitted)
            {
                var length = (int)message.GetVarULong();
                if (_tempBytes.Length < length)
                    Array.Resize(ref _tempBytes, length);

                var writeMemory = _buffer.GetMemory(length);
                message.GetBytes(length, _tempBytes);
                _tempBytes.CopyTo(writeMemory);
                _buffer.Advance(length);
                command = _commandSerializer.Deserialize<T>(_buffer.WrittenMemory);
                return true;
            }
            else
            {
                // read uid // 2^16 max
                ushort uid = message.GetUShort();

                var key = checked(((ushort)client, uid));
                if (!_splitMessages.TryGetValue(key, out var storage))
                    _splitMessages[key] = storage =
                        _queue.TryDequeue(out storage) ? storage : new(this);

                storage.Write(message);

                if (storage.Recieved(_buffer))
                {
                    command = _commandSerializer.Deserialize<T>(_buffer.WrittenMemory);
                    storage.Clear();
                    _splitMessages.Remove(key);
                    _queue.Enqueue(storage);
                    return true;
                }
                command = default;
                return false;
            }
        }

        public List<Message> GetMessages<T>(Command<T> data, List<Message> messages)
        {
            _buffer.Clear();
            _commandSerializer.Serialize(_buffer, ref data);
            var written = _buffer.WrittenSpan;
            Console.WriteLine($"Message size: {written.Length} bytes");
            if (written.Length >= SplitSize) // need to split
            {
                Console.WriteLine(written.Length);
                return GetSplitted<T>(written, messages);
            }
            else
            {
                return GetSingle<T>(written, messages);
            }
        }

        private List<Message> GetSplitted<T>(ReadOnlySpan<byte> written, List<Message> messages)
        {
            // write true if splitted
            // write true if last split index
            // write split index // 2^16 max
            // write uid // 2^16 max

            int splitCount = written.Length / SplitSize +
                (written.Length % SplitSize == 0 ? 0 : 1);
            if (splitCount > ushort.MaxValue)
                throw new NotSupportedException();
            var commandId = CommandsRegistry.GetId<T>();
            ushort uid = _splitMessageId++;
            for (int i = 0; i < splitCount; i++)
            {
                var splitMessage = Message.Create(MessageSendMode.Reliable, (ushort)commandId);
                splitMessage.AddBool(true); // split
                splitMessage.AddUShort(uid); // uid
                splitMessage.AddUShort((ushort)i); // index
                splitMessage.AddBool(i == splitCount - 1); // is last

                var splitWrite = written[(i * SplitSize)..];
                splitWrite = splitWrite[..Maths.Min(splitWrite.Length, SplitSize)];
                WriteToMessage(splitWrite, splitMessage);
                messages.Add(splitMessage);
            }
            return messages;
        }

        private List<Message> GetSingle<T>(ReadOnlySpan<byte> written, List<Message> messages)
        {
            var commandId = CommandsRegistry.GetId<T>();
            var message = Message.Create(MessageSendMode.Reliable, (ushort)commandId);
            message.AddBool(false);
            WriteToMessage(written, message);
            messages.Add(message);
            return messages;
        }

        private void WriteToMessage(ReadOnlySpan<byte> write, Message message)
        {
            int length = write.Length;
            if (length > _tempBytes.Length)
                Array.Resize(ref _tempBytes, length);
            write.CopyTo(_tempBytes);
            message.AddBytes(_tempBytes, 0, length);
        }

        private class SplitMessageStorage
        {
            private ushort? _lastIndex;
            private readonly ArrayBufferWriter<byte> _allocator;
            private readonly SortedDictionary<ushort, Memory<byte>> _splitMessageData;
            private readonly MessageIO _owner;

            public SplitMessageStorage(MessageIO messageIO)
            {
                _allocator = new();
                _splitMessageData = new();
                _lastIndex = null;
                _owner = messageIO;
            }

            public bool Recieved(IBufferWriter<byte> writer)
            {
                if (_lastIndex.HasValue && _splitMessageData.Count == _lastIndex.Value + 1)
                {
                    foreach (var item in _splitMessageData)
                    {
                        var read = item.Value;
                        var write = writer.GetMemory(read.Length);
                        read.CopyTo(write);
                        writer.Advance(read.Length);
                    }
                    return true;
                }
                return false;
            }

            public void Clear()
            {
                _splitMessageData.Clear();
                _lastIndex = null;
                _allocator.Clear();
            }

            public void Write(Message message)
            {
                ushort index = message.GetUShort(); // index
                bool isLast = message.GetBool(); // is last
                if (isLast)
                {
                    _lastIndex = index;
                }
                var length = (int)message.GetVarULong();
                if (_owner._tempBytes.Length < length)
                    Array.Resize(ref _owner._tempBytes, length);
                message.GetBytes(length, _owner._tempBytes);
                var memory = _allocator.GetMemory(length);
                _splitMessageData[index] = memory[..length];
                var from = _owner._tempBytes.AsSpan(0, length);
                from.CopyTo(memory.Span);
                _allocator.Advance(length);
            }
        }
    }
}
