using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    public class MuseumOpener : MonoBehaviour
    {
        public void Open(bool pressed)
        {
            if(!pressed)
            {
                return;
            }
            Debug.Log("Open Museum Button Pressed!");
            GameManager.Instance.OpenMuseum();
        }
    }
}
