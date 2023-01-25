using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
 
    /// <summary>
    /// Sets the color based on the horizontal scale of the UI object
    /// </summary>
    public class ColorByScale : MonoBehaviour
    {

        [Header("Parameters")]
        
        [SerializeField]
        private Color zeroColor;
        
        [SerializeField]
        private Color fullColor;



        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();

        }

        // Update is called once per frame
        void Update()
        {
            float scale = transform.localScale.x;

            Color color = Color.Lerp(zeroColor, fullColor, scale);
            image.color = color;
        }
    }
}
