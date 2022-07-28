using System.Collections.Generic;
using Cyens.ReInherit.Extensions;
using UnityEngine;

namespace Cyens.ReInherit.Pooling
{
    public class PrefabPoolData : ScriptableObject
    {
        private static bool _isCreating;

        private static Transform _commonParent;
        private static Transform _defaultTransform;

        private readonly List<MonoBehaviour> m_allItems = new();
        private readonly List<MonoBehaviour> m_pool = new();

        private Transform m_gatheredTransform;
        private MonoBehaviour m_prototype;

        private void Awake()
        {
            if (_isCreating) {
                return;
            }

            Debug.LogError(
                $"{nameof(PrefabPoolData)} must be created only through the {nameof(Create)} static function"
            );
            Destroy(this);
        }

        private void OnDestroy()
        {
            if (m_gatheredTransform != null) {
                Destroy(m_gatheredTransform.gameObject);
            }
        }

        private void MakeParent(string parentName)
        {
            if (_commonParent == null) {
                var commonParent = new GameObject("Gathered");
                commonParent.SetActive(false);
                DontDestroyOnLoad(commonParent);
                _commonParent = commonParent.transform;
            }

            if (string.IsNullOrEmpty(parentName)) {
                parentName = "Pool";
            }

            var obj = new GameObject(parentName);
            m_gatheredTransform = obj.transform;
            m_gatheredTransform.SetParent(_commonParent, false);
        }

        public static PrefabPoolData Create(MonoBehaviour prototype, int preconstruct, string parentName = null)
        {
            _isCreating = true;
            var pool = CreateInstance<PrefabPoolData>();
            _isCreating = false;

            pool.MakeParent(parentName);

            pool.m_prototype = prototype;
            pool.m_pool.ReserveCapacity(preconstruct);
            pool.m_allItems.ReserveCapacity(preconstruct);

            for (var i = 0; i < preconstruct; ++i) {
                var obj = Instantiate(prototype, pool.m_gatheredTransform, true);
                pool.m_allItems.Add(obj);
                pool.m_pool.Add(obj);
            }

        #if UNITY_EDITOR
            if (pool.m_pool.Count > 0) {
                pool.m_gatheredTransform.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
            } else {
                pool.m_gatheredTransform.gameObject.hideFlags |= HideFlags.HideInHierarchy;
            }
        #endif

            return pool;
        }

        public void Gather(MonoBehaviour obj)
        {
            if (obj == null) {
                return;
            }

            var transform = obj.transform;

            if (transform.parent == m_gatheredTransform) {
                return;
            }
        #if UNITY_EDITOR
            if (m_pool.Count == 0) {
                m_gatheredTransform.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
            }
        #endif
            // ReSharper disable once SuspiciousTypeConversion.Global
            (obj as IGatherableCallback)?.OnGather();

            transform.SetParent(m_gatheredTransform, false);
            m_pool.Add(obj);
        }

        public MonoBehaviour Spawn(Transform parentTransform = null, Vector3 position = default)
        {
            if (m_pool.TryPop(out var outValue)) {
                var newTransform = outValue.transform;
                newTransform.SetParent(parentTransform, false);
                outValue.gameObject.SetActive(true);
                newTransform.position = position;

            #if UNITY_EDITOR
                if (m_pool.Count == 0) {
                    m_gatheredTransform.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                }
            #endif

                return outValue;
            }

            // No objects left in the pool, we need to instantiate a new one.
            outValue = Instantiate(m_prototype, parentTransform, false);
            outValue.transform.position = position;

            // ReSharper disable once SuspiciousTypeConversion.Global
            (outValue as ISpawnableCallback)?.OnSpawn();

            m_allItems.Add(outValue);

            return outValue;
        }
    }
}