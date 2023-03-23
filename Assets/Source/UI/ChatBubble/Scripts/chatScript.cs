using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ChatScript : MonoBehaviour
    {
        [SerializeField] GameObject player;
        private GameObject ChatGameobject;

        // Start is called before the first frame update
        void Start()
        {
            //ChatBubble.Create(player, new Vector3(0, 3), ChatBubble.IconType.Angry, "Hello");
            //ChatGameobject = player.transform.Find("Characters Chat").gameObject;
            //ChatGameobject.GetComponent<ChatBubble>().Setup(ChatBubble.IconType.Happy, " ");

            ChatGameobject = player.transform.Find("CanvasChat").gameObject;
            ChatGameobject.GetComponentInChildren<VisitorChat>().Setup(VisitorChat.IconType.Angry);
        }
    }
}
