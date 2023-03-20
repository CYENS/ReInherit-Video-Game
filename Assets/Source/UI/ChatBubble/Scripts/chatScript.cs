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
            ChatBubble.Create(player, new Vector3(0, 3), ChatBubble.IconType.Angry, "Hello");
        }

       
    }
}
