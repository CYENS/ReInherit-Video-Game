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
    public class AnyGarbageVisible : ActionNode
    {

        [SerializeField] private float viewDistance = 8.0f;
        [SerializeField] private float viewAngle = 45.0f;
        [SerializeField] private LayerMask solidLayers;

        [SerializeField] private bool verbose = false;

        private Garbage[] garbages;
        private int index = 0;

        protected override void OnStart()
        {
            garbages = FindObjectsOfType<Garbage>();
            index = 0;
            if( verbose ) 
                Debug.Log("Trying to check garbage visibility. "+garbages.Length+" garbage piles detected.");
        }

        protected override void OnStop()
        {
            garbages = null;
        }


        protected bool isVisible( GameObject target )
        {
            Transform transform = context.transform;
            Vector3 myPos = transform.position + Vector3.up*0.25f;

            Vector3 targetPos = target.transform.position + Vector3.up * 0.25f;

            // Distance check
            if( Vector3.Distance(myPos, targetPos) > viewDistance ) return false;


            // View angle check
            Vector3 toTarget = targetPos - myPos;
            Vector3 forward = transform.forward;
            if( Vector3.Angle(forward,toTarget) > viewAngle ) return false;
        
            // Raycast check
            Ray ray = new Ray(myPos, toTarget.normalized );
            RaycastHit hit;
            if( Physics.Raycast(ray,out hit, viewDistance, solidLayers))
            {
                if(verbose) Debug.Log("... Raycast hit: "+hit.collider.gameObject);
                if( hit.collider.gameObject == target ) return true;
                return false;
            }

            // By default assume that the object is visible, since nothing was in the way
            // to obstruct it.
            return true;
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
            else 
            {
                if( verbose ) Debug.Log("..Garbage pile "+index+" is NOT visible");
            }

            // Check for the next pile next frame
            index++;
            return State.Running;
        }

        
    }
}
