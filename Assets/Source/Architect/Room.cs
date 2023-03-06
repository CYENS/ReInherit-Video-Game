using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

namespace Cyens.ReInherit.Architect
{
    public abstract class Room : ScriptableObject
    {
        public abstract RoomGraph Graph { get; }

        public abstract IReadOnlyList<Index> Tiles { get; }

        public abstract bool Contains(Index index);

        public bool ContainsPoint(Vector3 point) => Contains(Index.FromWorld(point));

        // public abstract void Add(Index index);
        //
        // public abstract void Remove(Index index);
    }
}