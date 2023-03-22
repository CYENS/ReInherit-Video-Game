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
        }

        [SerializeField] private Sprite happyIconSprite;
        [SerializeField] private Sprite neutralIconSprite;
        [SerializeField] private Sprite angryIconSprite;

        private Image iconImage;
        private Camera mainCamera;


        private void Awake()
        {
            iconImage = transform.Find("Icon").GetComponent<Image>();
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public void Setup(IconType iconType)
        {
            iconImage.sprite = GetIconSprite(iconType);
        }

        private Sprite GetIconSprite(IconType iconType)
        {
            switch (iconType)
            {
                default:
                case IconType.Happy: return happyIconSprite;
                case IconType.Neutral: return neutralIconSprite;
                case IconType.Angry: return angryIconSprite;
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }


    }
}
