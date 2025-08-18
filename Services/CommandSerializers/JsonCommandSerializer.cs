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
            var json = Encoding.UTF8.GetString(data.Span);
            Console.WriteLine($"Command<{typeof(T).Name}>:\n {json}");
            var cmd = JsonConvert.DeserializeObject<Command<T>>(json);
            Console.WriteLine("Deserialize " + cmd);
            return cmd;
        }

        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data) where T : ICommandData
        {
            var json = JsonConvert.SerializeObject(data);
            Console.WriteLine($"Serialize Command<{typeof(T).Name}>:\n {json}");
            buffer.Write(Encoding.UTF8.GetBytes(json));
        }
    }
}
