using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class JsonUTF8CommandSerializer : ICommandSerializer
    {
        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            return SerializationUTF8.Deserialize<Command<T>>(data);
        }

        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data)
        {
            SerializationUTF8.Serialize(data, buffer);
        }
    }
}