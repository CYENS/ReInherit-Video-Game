using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cyens.ReInherit.Managers;
using Cyens.ReInherit.Exhibition;


namespace Cyens.ReInherit
{
    /// <summary>
    /// Represents specifically an artifact in the player's inventory.
    /// It makes sure to gray out the icon if the artifact is already on display,
    /// or otherwise unavailable.
    /// </summary>
    public class ExhibitInventoryItem : InventoryItem
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("Reference to the image that grays out the base icon when item is unavailable")]
        private Image m_stripesImage;

        
        private void Update() 
        {
            int exhibitIndex = m_metaData.siblingIndex;
            var state = ExhibitManager.GetState(exhibitIndex);
            m_stripesImage.gameObject.SetActive( state != Exhibit.State.Storage );
        }

    }
}
