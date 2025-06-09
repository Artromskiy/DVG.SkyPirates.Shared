using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using ProtoBuf;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class ProtobufSerializer : ICommandSerializer
    {
        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data)
            where T : ICommandData
        {
            Serializer.Serialize(buffer, data);
        }

        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data)
            where T : ICommandData
        {
            Command<T> result;
            try
            {
                result = Serializer.Deserialize<Command<T>>(data);
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
