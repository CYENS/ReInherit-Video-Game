using Cyens.ReInherit.Pooling;
using UnityEngine;

namespace Cyens.ReInherit.Architect
{
    [DefaultExecutionOrder(int.MinValue)]
    public class ArchitectLibrary : MonoBehaviour
    {
        private static ArchitectLibrary _instance;

        [SerializeField] private RoomPrefabs roomPrefabs;

        public static PrefabPool<BlockModel> RoomPrefabs
        {
            get {
            #if UNITY_EDITOR
                _instance = FindObjectOfType<ArchitectLibrary>();
            #endif
                return _instance.roomPrefabs.RoomModelPool;
            }
        }
        
        private void Awake()
        {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            transform.parent = null;
            DontDestroyOnLoad(this);

            roomPrefabs.Init();
        }
        
        public static bool IsDisposing => _isQuitting;

        private static bool _isQuitting;

        private void Update()
        {
            _isQuitting = false;
        }

        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            Application.wantsToQuit += () => {
                _isQuitting = true;
                return true;
            };
        }
    }
}