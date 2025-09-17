using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public static class Serialization
    {
        private static readonly JsonSerializer _serializer;

        static Serialization()
        {
            _serializer = JsonSerializer.CreateDefault();
            _serializer.ContractResolver = new DictionaryAsArrayResolver();
        }

        public static string Serialize<T>(T obj)
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = _serializer.Formatting;
                _serializer.Serialize(jsonTextWriter, obj);
            }

            return stringWriter.ToString();
        }

        public static T Deserialize<T>(string json)
        {
            using JsonTextReader reader = new JsonTextReader(new StringReader(json));
            return _serializer.Deserialize<T>(reader);
        }
    }
}
