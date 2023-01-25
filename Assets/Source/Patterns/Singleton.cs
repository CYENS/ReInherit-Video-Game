using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit.Patterns
{
    /// <summary>
    /// There can be only one instance of this object and it is globally accessible by other classes.
    /// Classes that inherit the Singleton class will become singleton instances.
    /// Source for this code: https://www.unitygeek.com/unity_c_singleton/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<T> ();
                    if (instance == null) {
                        GameObject obj = new GameObject ();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        
        public virtual void Awake ()
        {
            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad (this.gameObject);
            } else {
                Destroy (gameObject);
            }
        }
    }

}