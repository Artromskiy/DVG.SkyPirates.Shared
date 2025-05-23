﻿using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Commands
{
    public static class CommandIds
    {
        private static readonly Dictionary<Type, int> _typeToId;
        private static readonly Dictionary<int, Type> _idToType;

        private static readonly Type[] _messages = new Type[]
        {
            typeof(RegisterSquad),
            typeof(UnregisterSquad),
            typeof(MoveSquad),
            typeof(RotateSquad),
            typeof(FixateSquad),
            typeof(RegisterSquadUnit),
            typeof(UnregisterSquadUnit),
        };

        static CommandIds()
        {
            _typeToId = new Dictionary<Type, int>();
            _idToType = new Dictionary<int, Type>();
            for (int i = 0; i < _messages.Length; i++)
            {
                int id = i + 1;
                _typeToId[_messages[i]] = id;
                _idToType[id] = _messages[i];
            }
        }

        public static int GetId<T>() => _typeToId[typeof(T)];
        public static Type GetCommand(ushort id) => _idToType[id];
    }
}
