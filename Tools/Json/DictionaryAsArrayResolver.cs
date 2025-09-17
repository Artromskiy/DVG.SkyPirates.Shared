using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class DictionaryAsArrayResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if (IsDictionary(objectType))
            {
                JsonArrayContract contract = base.CreateArrayContract(objectType);
                contract.OverrideCreator = (args) => CreateInstance(objectType);
                return contract;
            }

            return base.CreateContract(objectType);
        }

        private object CreateInstance(Type objectType)
        {
            Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(objectType.GetGenericArguments());
            return Activator.CreateInstance(dictionaryType);
        }

        private static bool IsDictionary(Type objectType)
        {
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                return true;
            }

            if (objectType.GetInterface(typeof(IDictionary<,>).Name) != null)
            {
                return true;
            }

            return false;
        }

    }
}
