using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.Text;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class JsonCommandSerializer : ICommandSerializer
    {
        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data) where T : ICommandData
        {
            var json = Encoding.UTF8.GetString(data.Span);
            return Serialization.Deserialize<Command<T>>(json);
        }

        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data) where T : ICommandData
        {
            var json = Serialization.Serialize(data);
            buffer.Write(Encoding.UTF8.GetBytes(json));
        }
    }
}
