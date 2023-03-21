using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cyens.ReInherit
{
    public class ChatBubble : MonoBehaviour
    {
        [SerializeField] private Sprite m_happyIconSprite;
        [SerializeField] private Sprite m_neutralIconSprite;
        [SerializeField] private Sprite m_angryIconSprite;
        [SerializeField] private Sprite m_disgustedIconSprite;
        [SerializeField] private Sprite m_boredIconSprite;

        private SpriteRenderer m_backgroundSpriteRenderer;
        private SpriteRenderer m_iconSpriteRenderer;
        private TextMeshPro m_textMeshPro;

        private Camera m_mainCamera;

        public static ChatBubble Create(Transform parent, Vector3 localPosition)
        {
            // Get the chat prefab from prefabs library
            PrefabsLibrary pl = ScriptableObject.CreateInstance<PrefabsLibrary>();
            GameObject chatBubblePrefab = pl.GetPrefab("Characters Chat");

            GameObject chatBubble = Instantiate(chatBubblePrefab, parent);
            chatBubble.transform.localPosition = localPosition;
            chatBubble.GetComponent<ChatBubble>().Setup(Visitor.Emotion.Angry, "Initialize");

            return chatBubble.GetComponent<ChatBubble>();
        }

        private void Awake()
        {
            m_backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
            m_iconSpriteRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
            m_textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        }

        private void Start()
        {
            m_mainCamera = Camera.main;
        }

        public void ShowBubble(bool show)
        {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(show);
            }
        }

        public void Setup(Visitor.Emotion emotion, string text)
        {
            m_textMeshPro.SetText(text);
            m_textMeshPro.ForceMeshUpdate();
            Vector2 textSize = m_textMeshPro.GetRenderedValues(false);
            Vector2 padding = new Vector2(7f, 7f);
            m_backgroundSpriteRenderer.size = padding;
            //Vector3 offset = new Vector3(-16f, 0.5f);
            //m_backgroundSpriteRenderer.size = textSize + padding;
            //m_backgroundSpriteRenderer.transform.localPosition = new Vector3(m_backgroundSpriteRenderer.size.x / 2f, 0f) + offset;
            m_iconSpriteRenderer.sprite = GetIconSprite(emotion);
            m_iconSpriteRenderer.transform.localPosition = m_backgroundSpriteRenderer.transform.localPosition;
        }

        private Sprite GetIconSprite(Visitor.Emotion iconType)
        {
            switch (iconType)
            {
                default:
                case Visitor.Emotion.Happy :return m_happyIconSprite;
                case Visitor.Emotion.Neutral: return m_neutralIconSprite;
                case Visitor.Emotion.Angry: return m_angryIconSprite;
                case Visitor.Emotion.Disgusted: return m_disgustedIconSprite;
                case Visitor.Emotion.Bored: return m_boredIconSprite;
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + m_mainCamera.transform.rotation * Vector3.forward, m_mainCamera.transform.rotation * Vector3.up);
        }
    }
}
