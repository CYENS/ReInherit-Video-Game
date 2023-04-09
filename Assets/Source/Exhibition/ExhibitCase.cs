using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Represents the case that holds the artifact.
    /// This script is used mostly as metadata holder for the exhibit case prefab.
    /// </summary>
    public class ExhibitCase : MonoBehaviour
    {

        [Header("Parameters")]

        [SerializeField]
        [Tooltip("How well protected the artifact is. 1 => artifact receives practically no damage.")]
        [Range(0.0f,1.0f)]
        private float damageProof = 0.5f;


        [SerializeField]
        [Tooltip("Affects how often the case will require cleaning")]
        [Range(0.0f,0.9f)]
        private float dirtProof = 0.1f;


        [SerializeField]
        [Tooltip("How much is the cost of this exhibit case")]
        private int price = 100;

        [Header("Gameplay")]

        [SerializeField]
        [Tooltip("How dirty the exhibit case is. The dirtier, the worse the view of the artifact will be.")]
        [Range(0.0f,1.0f)]
        private float dirt = 0.0f;


        [Header("References")]

        [SerializeField]
        [Tooltip("An object that will be used as a placement reference for the artifact")]
        private Transform placement;

        private void Start() 
        {
            
        }

        private void OnEnable() 
        {
            
            GameManager.Instance.whenRoundEnds += OnRoundEnd;
        }

        private void OnDisable() 
        {
            
            
            GameManager.Instance.whenRoundEnds -= OnRoundEnd;
        }

        /// <summary>
        /// Performs a set of action when the round ends.
        /// </summary>
        protected void OnRoundEnd(int round)
        {
            // Make the exhibit case dirtier at the end of each round
            float random = Random.Range(0.9f, 1.0f);
            float delta = random - dirtProof;
            dirt = Mathf.Clamp( dirt + delta, 0.0f, 1.0f );
        }

    }
}
