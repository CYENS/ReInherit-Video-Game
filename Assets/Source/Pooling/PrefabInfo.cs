using System;
using UnityEngine;

namespace Cyens.ReInherit.Pooling
{
    [Serializable]
    public struct PrefabInfo<T> where T : MonoBehaviour
    {
        [SerializeField] private T prefab;
        [SerializeField] private int preconstruct;
        [SerializeField, HideInInspector] private PrefabPool<T> pool;

        public PrefabPool<T> Pool => pool;

        public void Init()
        {
            if (pool is { IsInitialized: true }) {
                return;
            }

            pool = new PrefabPool<T>(prefab, preconstruct);
        }

        public void Destroy()
        {
            pool.Destroy();
        }
    }
}