using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Ghostify : MonoBehaviour
    {

        [SerializeField]
        private Color color;

        [Header("References")]
        public Material ghostMaterial;
        private Material ghostMaterialInstance;

        private new MeshRenderer renderer;

        private Material[] originalMaterials;
        private Material[] ghostMaterials;



        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            originalMaterials = renderer.sharedMaterials;

            // Create instance of ghost material
            renderer.material = ghostMaterial;
            ghostMaterialInstance = renderer.material;
            renderer.sharedMaterial = originalMaterials[0];

            // Assign the array with the ghost material instance
            ghostMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < ghostMaterials.Length; i++)
                ghostMaterials[i] = ghostMaterialInstance;

        }

        private void OnEnable()
        {
            renderer.sharedMaterials = ghostMaterials;
            ghostMaterialInstance.color = color;
        }

        private void OnDisable()
        {
            renderer.sharedMaterials = originalMaterials;
        }


        public void SetColor( Color color )
        {
            this.color = color;
            ghostMaterialInstance.color = color;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(Application.isPlaying)
                SetColor(color);
        }
#endif

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
