using Newtonsoft.Json;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public static class Serialization
    {
        private static readonly JsonSerializerSettings? _settings;
        static Serialization()
        {
            _settings = JsonConvert.DefaultSettings?.Invoke();
            if (_settings != null)
            {
                _settings.ContractResolver = new DictionaryAsArrayResolver();
            }
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
