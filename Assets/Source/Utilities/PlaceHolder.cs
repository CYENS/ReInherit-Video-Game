using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Cyens.ReInherit
{
    /// <summary>
    /// A script that deactivates the object if the game is running,
    /// or if it is placed inside a scene.
    /// 
    /// Useful in cases where we want a placeholder to be visible inside a prefab, 
    /// but not when placed in a scene, as it will be replaced with another object
    /// </summary>
    public class PlaceHolder : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            gameObject.SetActive(false);
        }

        #if UNITY_EDITOR
        
        private void OnValidate()
        {
            // Check if we are in a special editing stage
            if (StageUtility.GetStage(gameObject) != StageUtility.GetMainStage())
            {
                return;
            }

            // Check to see if the scene is valid
            if( gameObject.scene.IsValid() )
            {
                gameObject.SetActive(false);
            }
        }


        #endif
    }
}
