using System;
using System.Collections.Generic;
using System.Reflection;
using Cyens.ReInherit.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cyens.ReInherit.Pooling
{
    public abstract class ClassPool
    {
        protected static readonly MethodInfo CreateInstanceMethodInfo;

        /// Caches the reflection info for the CreateInstance method in ScriptableObject
        static ClassPool()
        {
            CreateInstanceMethodInfo = typeof(ScriptableObject).GetMethod(
                "CreateInstance", 1, BindingFlags.Static | BindingFlags.Public, null, new Type[] { }, null
            );
        }
    }

    [UsedImplicitly]
    public sealed class ClassPool<T> : ClassPool where T : class, new()
    {
        private ClassPool() { }
        
        [NotNull] private static readonly Func<T> OnSpawn;
        [NotNull] private static readonly Action<T> OnGather;

        /// Caches and selects the appropriate constructor based on whether T is
        /// descended from ScriptableObject or not, and whether T implements ISpawnableCallback
        static ClassPool()
        {
            Func<T> newT;
            if (typeof(T).IsSubclassOf(typeof(ScriptableObject))) {
                var generic = CreateInstanceMethodInfo.MakeGenericMethod(typeof(T));
                newT = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), generic);
            } else {
                newT = () => new T();
            }

            if (typeof(T).IsSubclassOf(typeof(ISpawnableCallback))) {
                OnSpawn = () => {
                    var outValue = newT();
                    ((ISpawnableCallback)outValue).OnSpawn();
                    return outValue;
                };
            } else {
                OnSpawn = newT;
            }

            if (typeof(T).IsSubclassOf(typeof(IGatherableCallback))) {
                OnGather = obj => {
                    Pool.Add(obj);
                    ((IGatherableCallback)obj).OnGather();
                };
            } else {
                OnGather = obj => { Pool.Add(obj); };
            }
        }

        private static readonly List<T> Pool = new();

        public static int PoolCount => Pool.Count;

        public static T Spawn()
        {
        #if UNITY_EDITOR
            if (!typeof(T).IsSubclassOf(typeof(ScriptableObject)) && typeof(T).IsSubclassOf(typeof(Object))) {
                Debug.LogWarning($"Unity Objects should not be used with {nameof(ClassPool<T>)}");
            }
        #endif
            return Pool.TryPop(out var outValue) ? outValue : OnSpawn();
        }

        public static void Gather(T obj)
        {
        #if UNITY_EDITOR
            if (!typeof(T).IsSubclassOf(typeof(ScriptableObject)) && typeof(T).IsSubclassOf(typeof(Object))) {
                Debug.LogWarning($"Unity Objects should not be used with {nameof(ClassPool<T>)}");
            }
        #endif
            if (obj == null) {
                Debug.LogError($"Cannot gather null object for {nameof(ClassPool<T>)}");
                return;
            }

            OnGather(obj);
        }

        public static int Count => Pool.Count;

        public static void ReserveCapacity(int capacity)
        {
            Pool.ReserveCapacity(capacity);
        }

        public static void Prespawn(int count)
        {
            Pool.ReserveCapacity(count);
            for (var i = Pool.Count; i < count; ++i) {
                Pool.Add(OnSpawn());
            }
        }
    }
}