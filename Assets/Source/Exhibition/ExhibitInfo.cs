using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit.Exhibition
{
    /// <summary>
    /// Holds information about the appearance of the exhibit, the artifact model inside of it,
    /// as well as game related parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "ExhibitInfo", menuName = "Data/ExhibitInfo", order = 0)]
    public class ExhibitInfo : ScriptableObject 
    {
        
        [System.Serializable]
        public struct Case 
        {
            public UnityEngine.Object prefab;
            public float price;
        }


        [Header("Prefabs")]
        [Tooltip("Contains the upgrades of the exhibit cases (In ascending order)")]
        public UnityEngine.Object[] m_cases;

        [Tooltip("The model of the artifact that will be placed in the exhibit case")]
        public UnityEngine.Object m_artifact;


        [Header("Gameplay")]

        public float test;


    }

}
