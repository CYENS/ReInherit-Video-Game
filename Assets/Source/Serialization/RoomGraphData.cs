using System;
using System.Collections.Generic;
using Cyens.ReInherit.Architect;
using Index = Cyens.ReInherit.Architect.Index;

namespace Cyens.ReInherit.Serialization
{
    [Serializable]
    public class RoomJsonData
    {
        public List<Index> blocks = new();
        public List<KeyValuePair<Index, Direction>> connections = new();
    }

    [Serializable]
    public class RoomGraphJsonData
    {
        public List<RoomJsonData> rooms = new();
    }
}