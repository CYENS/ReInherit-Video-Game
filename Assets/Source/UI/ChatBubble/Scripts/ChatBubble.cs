using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cyens.ReInherit
{
    public class ChatBubble : MonoBehaviour
    {
        public enum IconType
        {
            Happy,
            Neutral,
            Angry,
            //Disgusted,
        }

        [SerializeField] private Sprite happyIconSprite;
        [SerializeField] private Sprite neutralIconSprite;
        [SerializeField] private Sprite angryIconSprite;
        //[SerializeField] private Sprite disgustedIconSprite;

        private SpriteRenderer backgroundSpriteRenderer;
        private SpriteRenderer iconSpriteRenderer;
        private TextMeshPro textMeshPro;

        private Camera mainCamera;

        public static void Create(Transform parent, Vector3 localPosition, IconType iconType, string text)
        {
            // Get the chat prefab from prefabs library
            PrefabsLibrary pl = ScriptableObject.CreateInstance<PrefabsLibrary>();
            GameObject chatBubblePrefab = pl.GetPrefab("Characters Chat");

            GameObject chatBubble = Instantiate(chatBubblePrefab, parent);
            chatBubble.transform.localPosition = localPosition;

            chatBubble.transform.GetComponent<ChatBubble>().Setup(iconType, text);

            Destroy(chatBubble, 12f);
        }

        private void Awake()
        {
            backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
            iconSpriteRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
            textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public void Setup(IconType iconType, string text)
        {
            textMeshPro.SetText(text);
            textMeshPro.ForceMeshUpdate();
            Vector2 textSize = textMeshPro.GetRenderedValues(false);
            Debug.Log(textSize);
            Vector2 padding = new Vector2(8f, 2f);
            Vector3 offset = new Vector3(-16f, 0f);
            backgroundSpriteRenderer.size = textSize + padding;
            //backgroundSpriteRenderer.transform.localPosition = new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

            iconSpriteRenderer.sprite = GetIconSprite(iconType);
            iconSpriteRenderer.transform.localPosition =  backgroundSpriteRenderer.transform.localPosition;

        }

        private Sprite GetIconSprite(IconType iconType)
        {
            switch (iconType)
            {
                default:
                case IconType.Happy:return happyIconSprite;
                case IconType.Neutral: return neutralIconSprite;
                case IconType.Angry: return angryIconSprite;
                //case IconType.Disgusted: return disgustedIconSprite;
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
       

    }
}
