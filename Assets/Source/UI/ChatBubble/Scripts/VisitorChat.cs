using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class VisitorChat : MonoBehaviour
    {
        public enum IconType
        {
            Happy,
            Neutral,
            Angry,
            Disgusted,
            Bored
        }

        [SerializeField] private Sprite m_happyIconSprite;
        [SerializeField] private Sprite m_neutralIconSprite;
        [SerializeField] private Sprite m_angryIconSprite;
        [SerializeField] private Sprite m_disgustedIconSprite;
        [SerializeField] private Sprite m_boredIconSprite;

        private Image m_iconImage;
        private Camera m_mainCamera;


        private void Awake()
        {
            m_iconImage = transform.Find("Icon").GetComponent<Image>();
        }

        private void Start()
        {
            m_mainCamera = Camera.main;
            transform.parent.GetComponent<Canvas>().worldCamera = m_mainCamera;
        }

        public void Setup(Visitor.Emotion iconType)
        {
            m_iconImage.sprite = GetIconSprite(iconType);
        }

        public void ShowBubble(bool show)
        {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(show);
            }
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
        
        public void Setup(IconType iconType)
        {
            m_iconImage.sprite = GetIconSprite(iconType);
        }

        private Sprite GetIconSprite(IconType iconType)
        {
            switch (iconType)
            {
                default:
                case IconType.Happy :return m_happyIconSprite;
                case IconType.Neutral: return m_neutralIconSprite;
                case IconType.Angry: return m_angryIconSprite;
                case IconType.Disgusted: return m_disgustedIconSprite;
                case IconType.Bored: return m_boredIconSprite;
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + m_mainCamera.transform.rotation * Vector3.forward, 
                m_mainCamera.transform.rotation * Vector3.up);
        }


    }
}
