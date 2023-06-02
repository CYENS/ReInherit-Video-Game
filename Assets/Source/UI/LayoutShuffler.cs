using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Randomly shuffles the child nodes of a game object
    /// </summary>
    public class LayoutShuffler : MonoBehaviour
    {

        // Start is called before the first frame update
        void OnEnable()
        {
            int childCount = transform.childCount;
            for( int i=0; i<childCount; i++ )
            {
                int r = Random.Range(i, childCount);
                var child = transform.GetChild(i);
                child.SetSiblingIndex(r);
            }
            
        }

    }
}
