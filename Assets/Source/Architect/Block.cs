using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    // public class Block : ScriptableObject
    // {
    //     private Index m_index;
    //     public Index Index => m_index;
    //
    //     private BlockModel m_model;
    //     private Room m_parent;
    //
    //     public BlockModel Model => m_model;
    //     public Room Parent => m_parent;
    //     public RoomGraph Graph => m_parent.Graph;
    //
    //     private readonly DirectionList<Room> m_connections = new();
    //
    //     public Room GetConnection(Direction direction)
    //     {
    //         return m_connections[direction];
    //     }
    //
    //     public void Connect(Direction direction)
    //     {
    //         var index = m_index.Towards(direction);
    //         var otherBlock = Graph.GetBlockAtIndex(index);
    //         if (otherBlock == null || otherBlock.m_parent == m_parent) {
    //             return;
    //         }
    //
    //         m_connections[direction] = otherBlock.m_parent;
    //         otherBlock.m_connections[direction.Opposite] = m_parent;
    //
    //         otherBlock.RefreshModel();
    //         RefreshModel();
    //     }
    //
    //     public void Disconnect(Direction direction)
    //     {
    //         var index = m_index.Towards(direction);
    //         var otherBlock = Graph.GetBlockAtIndex(index);
    //         m_connections[direction] = null;
    //         if (otherBlock != null) {
    //             otherBlock.m_connections[direction.Opposite] = null;
    //             otherBlock.RefreshModel();
    //         }
    //
    //         RefreshModel();
    //     }
    //
    //     public static Block Create(Room parent, Index index)
    //     {
    //         if (parent == null || parent.Graph == null) {
    //             return null;
    //         }
    //
    //         var blockOut = CreateInstance<Block>();
    //         blockOut.m_index = index;
    //         blockOut.m_parent = parent;
    //
    //         blockOut.m_model = ArchitectLibrary.RoomPrefabs.Spawn(parent.Graph.transform, index.WorldCenter);
    //
    //         parent.Add(blockOut);
    //         parent.Graph.AddBlock(blockOut);
    //
    //         return blockOut;
    //     }
    //     
    //     public void RemoveAndGather()
    //     {
    //         var parent = m_parent;
    //         if (parent == null) {
    //             return;
    //         }
    //         
    //         ArchitectLibrary.RoomPrefabs.Gather(Model);
    //         m_parent = null;
    //         m_model = null;
    //         m_index = new Index(-1, -1);
    //         
    //         parent.Graph.NotifyBlockRemoved(m_index);
    //         parent.RefreshAll();
    //     }
    //
    //     private void OnDestroy()
    //     {
    //         if (!ArchitectLibrary.IsDisposing) {
    //             ArchitectLibrary.RoomPrefabs.Gather(Model);
    //         }
    //     }
    //
    //     public void RefreshModel()
    //     {
    //         var graph = m_parent.Graph;
    //         foreach (var direction in Direction.Values) {
    //             var indexTowards = m_index.Towards(direction);
    //             var blockTowards = graph.GetBlockAtIndex(indexTowards);
    //
    //             WallModel.WallType wallType;
    //
    //             if (blockTowards == null) {
    //                 wallType = WallModel.WallType.Wall;
    //             } else if (m_connections[direction] != null) {
    //                 wallType = WallModel.WallType.Door;
    //             } else if (blockTowards.m_parent == m_parent) {
    //                 wallType = WallModel.WallType.None;
    //             } else {
    //                 wallType = WallModel.WallType.Wall;
    //             }
    //
    //             Model.SetModelType(direction, wallType);
    //         }
    //     }
    //     
    //     public void ValidateConnections()
    //     {
    //         foreach (var direction in Direction.Values) {
    //             var connection = m_connections[direction];
    //             var blockTowards = Graph.GetBlockAtIndex(m_index.Towards(direction));
    //             if (connection == null) {
    //                 if (blockTowards != null && blockTowards.m_connections[direction.Opposite] != null) {
    //                     blockTowards.Disconnect(direction.Opposite);
    //                 }
    //                 continue;
    //             }
    //             
    //             if (blockTowards.m_connections[direction.Opposite] != m_parent || connection != blockTowards.Parent) {
    //                 Disconnect(direction);
    //             }
    //         }
    //     }
    // }
}