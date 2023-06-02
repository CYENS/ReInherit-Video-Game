using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    public class RatingBox : NumberBox
    {
        
        private void Awake()
        {
            m_mode = Mode.Float;
        }

        protected new void Update() 
        {

            // Get number amount from Game Manager
            float rating = GameManager.Rating;
            SetAmount(rating);

            // Everything else should work the same
            base.Update();
        }
    }
}
