using System;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    public class Block : ScriptableObject
    {
        private Index m_index;
        public Index Index => m_index;

        private BlockModel m_model;
        private Room m_parent;

        public BlockModel Model => m_model;
        public Room Parent => m_parent;
        public RoomGraph Graph => m_parent.Graph;

        public static Block Create(Room parent, Index index)
        {
            if (parent == null || parent.Graph == null) {
                return null;
            }

            var blockOut = CreateInstance<Block>();
            blockOut.m_index = index;
            blockOut.m_parent = parent;

            blockOut.m_model = ArchitectLibrary.RoomPrefabs.Spawn(parent.Graph.transform, index.WorldCenter);
            
            
            parent.Add(blockOut);
            parent.Graph.AddBlock(blockOut);

            blockOut.m_model.RecreateNavMesh();
            
            return blockOut;
        }

        private void OnDestroy()
        {
            if (m_model != null)
                m_model.DisableFloorAndRemoveNavMesh();
            
            if (!ArchitectLibrary.IsDisposing) {
                ArchitectLibrary.RoomPrefabs.Gather(Model);
            }
        }
    }
}