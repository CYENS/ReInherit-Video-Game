using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public abstract class VisibilityNode : ActionNode
    {
        [SerializeField] protected float viewDistance = 8.0f;
        [SerializeField] protected float viewAngle = 45.0f;
        [SerializeField] protected LayerMask solidLayers;

        protected bool isVisible( GameObject target)
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
                //if(verbose) Debug.Log("... Raycast hit: "+hit.collider.gameObject);
                if( hit.collider.gameObject == target ) return true;
                return false;
            }

            // By default assume that the object is visible, since nothing was in the way
            // to obstruct it.
            return true;
        }
    }
}
