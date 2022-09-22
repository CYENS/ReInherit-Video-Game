using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class TurnAround : ActionNode
    {

        [SerializeField] private float minRotation = 30.0f;
        [SerializeField] private float maxRotation = 90.0f;

        [SerializeField] private bool clockWise = true;
        [SerializeField] private bool antiClockWise = true;


        [SerializeField] private float rotationSpeed = 5.0f;
        



        private float rotation;
        private Vector3 target;


        protected override void OnStart()
        {
            rotation = Random.Range(minRotation,maxRotation);
            if( clockWise && antiClockWise ) rotation = Random.value < 0.5f ? rotation : -rotation;
            else if( clockWise ) rotation = rotation + float.Epsilon;
            else if( antiClockWise ) rotation = -rotation;
            else rotation = 0.0f;

            target = Quaternion.Euler(Vector3.up*rotation) * context.transform.forward;

            
            Debug.Log("Rotating towards new forward");
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            Vector3 forward = Vector3.RotateTowards(
                context.transform.forward, target,
                Time.deltaTime*rotationSpeed, 0.0f);


            context.transform.rotation = Quaternion.LookRotation(forward);

            if( Vector3.Angle(context.transform.forward, target) < float.Epsilon ) return State.Success;
            return State.Running;
        }
    }
}
