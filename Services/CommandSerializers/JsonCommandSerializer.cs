using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Text;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class JsonCommandSerializer : ICommandSerializer
    {
        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data) where T : ICommandData
        {
            return JsonConvert.DeserializeObject<Command<T>>(Encoding.UTF8.GetString(data.Span));
        }

        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data) where T : ICommandData
        {
            buffer.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
        }
    }
}
