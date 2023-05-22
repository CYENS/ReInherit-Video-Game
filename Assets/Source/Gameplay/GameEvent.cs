using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{

    /// <summary>
    /// Data object containing the assets and metadata of an event that will be
    /// shown at the end of a round.
    /// </summary>   
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Data/GameEvent", order = 0)]
    public class GameEvent : ScriptableObject 
    {
        /// <summary>
        /// The type of event:
        /// - None: No event will be shown
        /// - Artifact: Ministry of Antiquities gives the museum a new artifact to exhibit
        /// - Expand: The office of city planning has decided to give you more area to build your museum with
        /// - Grant: You have acquired a grant
        /// - Award: You have won an award for best museum
        /// - Quiz: Trouble makers have mixed in fake artifacts with the ones from the Ministry of Antiquities. Use your expertise to figure which one is authentic.
        /// - Damage: Some sort of damage (plumbing, lights, etc.) causes you to give away money
        /// - Donations: Generous donations from various foundations
        /// </summary>
        public EventCategory category;


        [TextArea(3,5)]
        [Tooltip("Adds further context to the event")]
        public string comment;



        [Header("Conditions")]

        [Range(0.0f,5.0f)]
        [Tooltip("Your museum needs to have at least this much in ratings for the event to show")]
        public float minRating = 0.0f;

        [Range(0,1000)]
        [Tooltip("This much money is required for the event to succeed")]
        public int minFunding = 0;

        [Range(0.0f,1.0f)]
        [Tooltip("Demand a certain level of cleanliness. Zero means dirty. Whereas one is reserved for spotless")]
        public float minCleanliness;

        [Range(0.0f,1.0f)]
        [Tooltip("Demand a certain level of organization and theming")]
        public float minTheming;

        [Range(0.0f,1.0f)]
        [Tooltip("Demand a certain level of variety and diversity in exhibits and art pieces")]
        public float minVariety;

        [Range(0.0f,1.0f)]
        [Tooltip("Demand that the museum isn't crowded")]
        public float minSpacing;

        [Header("Prefabs")]

        [Tooltip("This is shown to players when they passed the condition")]
        public UnityEngine.Object whenPass;

        [Tooltip("This is shown to players when the conditions are not met")]
        public UnityEngine.Object whenFail;

    }

}
