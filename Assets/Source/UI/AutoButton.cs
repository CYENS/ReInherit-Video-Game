using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cyens.ReInherit.UI {
    public abstract class AutoButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
        private Image m_image;
        private Button m_button;
        private bool m_isPressed;

        protected Button Button {
            get {
                if (m_button == null) {
                    return m_button = GetComponent<Button>();
                }

                return m_button;
            }
        }

        protected Image Image {
            get {
                if (m_image == null) {
                    return m_image = GetComponent<Image>();
                }

                return m_image;
            }
        }


        public bool IsVisible {
            get => Image.enabled;
            set {
                if (Image.enabled != value) {
                    Image.enabled = value;
                    m_isPressed = false;
                }
            }
        }

        public bool IsInteractable {
            get => Button.interactable;
            set {
                if (Button.interactable != value) {
                    Button.interactable = value;
                    m_isPressed = false;
                }
            }
        }

        public bool IsPressed => m_isPressed;

        protected virtual void OnDisable() {
            m_isPressed = false;
        }

        protected virtual void OnEnable() {
            m_isPressed = false;
        }

        public void OnPointerDown(PointerEventData eventData) {
            m_isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData) {
            m_isPressed = false;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (IsInteractable) {
                OnClick();
            }
        }

        protected abstract void OnClick();
    }
}