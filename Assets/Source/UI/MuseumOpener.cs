using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{
    public class MuseumOpener : MonoBehaviour
    {
        public void Open()
        {
            GameManager.Instance.OpenMuseum();
        }
    }
}
