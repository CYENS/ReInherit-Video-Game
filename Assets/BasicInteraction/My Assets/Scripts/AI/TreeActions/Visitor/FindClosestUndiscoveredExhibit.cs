using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Checks to see if any exhibits that weren't visited before
    /// are visible.
    /// </summary>
    public class FindClosestUndiscoveredExhibit : VisibilityNode
    {
        //private Exhibit[] exhibits;
        //private HashSet<Exhibit> visited;
        //private HashSet<Exhibit> discovered;


        protected override void OnStart()
        {
            //if( exhibits == null ) exhibits = FindObjectsOfType<Exhibit>();
            //if( visited == null ) visited = new HashSet<Exhibit>();
            //if( discovered == null ) discovered = new HashSet<Exhibit>();

            Debug.Log("Finding Closest Visible Undiscovered Exhibit");

        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            //// Handle case where all exhibits are discovered and seen
            //if( visited.Count >= exhibits.Length )
            //{
            //    blackboard.exhibit = null;
            //    return State.Success;
            //}

            //// First: Try to discover any new exhibits
            //foreach( var exhibit in exhibits )
            //{
            //    // Ignore already visited exhibits
            //    if( visited.Contains(exhibit) ) continue;

            //    // Ignore already discovered exhibits
            //    if( discovered.Contains(exhibit) ) continue;

            //    // Do not discover non-visible exhibits
            //    if( isVisible(exhibit.gameObject) == false ) continue;

            //    discovered.Add(exhibit);
            //}


            //// Second: Find the closest discovered exhibit
            //float minDistance = float.MaxValue;
            //Exhibit closest = null;
            //foreach( var exhibit in exhibits )
            //{
            //    // Ignore already visited exhibits
            //    if( visited.Contains(exhibit) ) continue;

            //    if( discovered.Contains(exhibit) == false ) continue;

            //    float distance = Vector3.Distance(context.transform.position, exhibit.transform.position);
            //    if( distance < minDistance )
            //    {
            //        minDistance = distance;
            //        closest = exhibit;
            //    }
            //}

            //Debug.Log("Closest Exhibit: "+closest);

            //blackboard.exhibit = closest != null ? closest.gameObject : null;
            //if( closest != null ) visited.Add(closest);

            //return closest == null ? State.Failure : State.Success;
            return State.Running;
        }
    }
}
