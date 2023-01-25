using Cyens.ReInherit.Pooling;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [CreateAssetMenu(menuName = "Libraries/RoomLibrary")]
    public class RoomPrefabs : ScriptableObject
    {
        [SerializeField] private PrefabInfo<BlockModel> roomModelInfo;

        public PrefabPool<BlockModel> RoomModelPool => roomModelInfo.Pool;

        public void Init()
        {
            roomModelInfo.Init();
        }
    }
}