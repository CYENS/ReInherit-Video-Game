using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class chatScript : MonoBehaviour
    {
        [SerializeField] Transform player;

        // Start is called before the first frame update
        void Start()
        {
            ChatBubble.Create(player, new Vector3(3f, 3f), ChatBubble.IconType.Happy, "Here is some text");
        }

       
    }
}
