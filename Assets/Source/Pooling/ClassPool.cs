using System.Collections.Generic;
using Cyens.ReInherit.Extensions;
using UnityEngine;

namespace Cyens.ReInherit.Pooling
{
    public static class ClassPool<T> where T : new()
    {
        private static readonly List<T> Pool = new();

        public static int PoolCount => Pool.Count;

        public static int Count => Pool.Count;

        public static T Spawn()
        {
        #if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Object))) {
                Debug.LogWarning($"Unity Objects should not be used with {nameof(ClassPool<T>)}");
            }
        #endif

            var outValue = Pool.TryPop(out var value) ? value : new T();
            var spawnable = outValue as ISpawnableCallback;
            spawnable?.OnSpawn();
            return outValue;
        }

        public static void Gather(T obj)
        {
        #if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Object))) {
                Debug.LogWarning($"Unity Objects should not be used with {nameof(ClassPool<T>)}");
            }
        #endif

            if (obj == null) {
                Debug.LogWarning($"Cannot gather null object for {nameof(ClassPool<T>)}");
                return;
            }

            (obj as IGatherableCallback)?.OnGather();
            Pool.Add(obj);
        }

        public static void ReserveCapacity(int capacity)
        {
            Pool.ReserveCapacity(capacity);
        }

        public static void Prespawn(int count)
        {
            Pool.ReserveCapacity(count);
            for (var i = Pool.Count; i < count; ++i) {
                Pool.Add(new T());
            }
        }
    }
}