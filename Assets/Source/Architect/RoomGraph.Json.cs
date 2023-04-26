using Cyens.ReInherit.Serialization;

namespace Cyens.ReInherit.Architect
{
    public partial class RoomGraph
    {
        public void ClearAll()
        {
            foreach (var room in rooms) {
                foreach (var tile in room.tiles) {
                    var block = m_blocks[tile];
                    block.ClearBlockAndGatherModel();
                }
                
                Destroy(room);
            }
            
            rooms.Clear();
        }
        
        public RoomGraphJsonData WriteJsonData()
        {
            var data = new RoomGraphJsonData();
            foreach (var room in rooms) {
                data.rooms.Add(room.WriteJsonData());
            }
            
            return data;
        }

        public void LoadJsonData(RoomGraphJsonData data)
        {
            ClearAll();

            foreach (var roomData in data.rooms) {
                var newRoom = InnerRoom.Create(this);
                rooms.Add(newRoom);
                newRoom.LoadJsonData(roomData);
            }

            foreach (var room in rooms) {
                room.RefreshAll();
            }
        }
    }
}