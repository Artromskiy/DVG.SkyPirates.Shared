using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Messages
{
    public static class MessageIds
    {
        private static readonly Dictionary<Type, ushort> _typeToId;
        private static readonly Dictionary<ushort, Type> _idToType;

        private static readonly Type[] _messages = new Type[]
        {
            typeof(SyncAllUnits),
            typeof(RegisterUnit),
            typeof(UnitUnregister),
            typeof(UnitGhost),
        };

        static MessageIds()
        {
            _typeToId = new Dictionary<Type, ushort>();
            _idToType = new Dictionary<ushort, Type>();
            for (int i = 0; i < _messages.Length; i++)
            {
                ushort id = (ushort)(i + 1);
                _typeToId[_messages[i]] = id;
                _idToType[id] = _messages[i];
            }
        }

        public static ushort GetMessageId<T>() => _typeToId[typeof(T)];
        public static Type GetMessageType(ushort id) => _idToType[id];
    }
}
