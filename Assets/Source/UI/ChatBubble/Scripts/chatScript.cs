using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ChatScript : MonoBehaviour
    {
        [SerializeField] Transform player;

        // Start is called before the first frame update
        void Start()
        {
            ChatBubble chat = ChatBubble.Create(player, new Vector3(0, 3));
            chat.Setup(Visitor.Emotion.Angry, "Hello", true);
        }
    }
}
