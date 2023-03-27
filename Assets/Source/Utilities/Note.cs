using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Note : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("What the note will say")]
        [TextArea(3,5)]
        private string m_note;
    }
}
