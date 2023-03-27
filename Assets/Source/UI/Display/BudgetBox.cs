using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class BudgetBox : NumberBox
    {
        protected override void RefreshText()
        {
            base.RefreshText();
            m_textbox.text = "€" + m_actual.ToString();
        }

        protected new void Update() 
        {
            // Get number amount from Game Manager
            int funds = GameManager.Funds;
            SetAmount(funds);

            // Everything else should work the same
            base.Update();
        }
    }
}
