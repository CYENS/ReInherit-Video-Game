using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cyens.ReInherit.Pooling
{
    [Serializable]
    public abstract class PrefabPool
    {
        protected PrefabPoolData prefabPoolData;

        protected PrefabPool(MonoBehaviour prototype, int preconstruct, string parentName)
        {
            prefabPoolData = PrefabPoolData.Create(prototype, preconstruct, parentName);
        }

        protected virtual Type InnerType => typeof(MonoBehaviour);

        public void Destroy()
        {
            Object.Destroy(prefabPoolData);
        }

        public MonoBehaviour Spawn(Transform parent = null, Vector3 position = default)
        {
            return prefabPoolData.Spawn(parent, position);
        }

        public bool DynamicGather(MonoBehaviour behaviour)
        {
            if (behaviour.GetType() != InnerType) {
                return false;
            }

            prefabPoolData.Gather(behaviour);
            return true;
        }
    }

    [Serializable]
    public class PrefabPool<T> : PrefabPool where T : MonoBehaviour
    {
        public PrefabPool(T prototype, int preconstruct) : base(prototype, preconstruct, typeof(T).Name) { }

        protected override Type InnerType => typeof(T);

        public bool IsInitialized => prefabPoolData != null;

        public void Gather(T obj)
        {
            if (prefabPoolData != null) {
                prefabPoolData.Gather(obj);
            }
        }

        public new T Spawn(Transform parent = null, Vector3 position = default)
        {
            return (T)prefabPoolData.Spawn(parent, position);
        }

        public T Spawn(string name, Transform parent = null, Vector3 position = default)
        {
            var instance = Spawn(parent, position);
            instance.name = name;
            return instance;
        }

        public T SpawnLocal(Transform parent = null, Vector3 position = default)
        {
            var spawned = (T)prefabPoolData.Spawn(parent);
            spawned.transform.localPosition = position;
            return spawned;
        }

        public T SpawnLocal(string name, Transform parent = null, Vector3 position = default)
        {
            var instance = Spawn(parent);
            instance.name = name;
            instance.transform.localPosition = position;
            return instance;
        }

        protected bool Equals(PrefabPool<T> other)
        {
            return Equals(prefabPoolData, other.prefabPoolData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            return obj.GetType() == GetType() && Equals((PrefabPool<T>)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return prefabPoolData != null ? prefabPoolData.GetHashCode() : 0;
        }
    }
}