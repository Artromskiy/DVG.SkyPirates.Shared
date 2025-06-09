using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using MessagePack;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class MessagePackCommandSerializer : ICommandSerializer
    {
        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data)
            where T : ICommandData
        {
            MessagePackSerializer.Serialize(buffer, data);
        }

        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data)
            where T : ICommandData
        {
            Command<T> result;
            try
            {
                result = MessagePackSerializer.Deserialize<Command<T>>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(typeof(T));
                throw e;
            }
            return result;
        }
    }
}
