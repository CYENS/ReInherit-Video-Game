using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    /// <summary>
    /// An action node that succeeds if any garbage pile is visible from the point of view
    /// of this actor.
    /// </summary>
    public class AnyGarbageVisible : VisibilityNode
    {
        [SerializeField] private bool verbose = false;

        private Garbage[] garbages;
        private int index = 0;

        protected override void OnStart()
        {
            garbages = FindObjectsOfType<Garbage>();
            index = 0;
            // if( verbose ) Debug.Log("Trying to check garbage visibility. "+garbages.Length+" garbage piles detected.");
        }

        protected override void OnStop()
        {
            garbages = null;
        }

        protected override State OnUpdate()
        {
            if( index >= garbages.Length ) 
            {
                Debug.Log("No more garbage to check for");
                return State.Failure;
            }

            // Perform this check, just in case the garbage script/gameobject was destroyed.
            if( garbages[index] == null || garbages[index].gameObject == null )
            {
                index++;
                return State.Running;
            } 

            Garbage garbage = garbages[index];
            if( isVisible(garbage.gameObject) )
            {
                if( verbose ) Debug.Log("..Garbage pile "+index+" detected!");
                return State.Success;
            }

            // Check for the next pile next frame
            index++;
            return State.Running;
        }

        
    }
}
