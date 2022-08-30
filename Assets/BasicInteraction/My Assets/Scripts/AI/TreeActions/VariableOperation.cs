using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class VariableOperation : ActionNode
    {
        public enum Mode { Set = 0, Add = 1, Mult = 2 }

        [Tooltip("The operation type")]    
        [SerializeField] private Mode mode = Mode.Set;

        [Tooltip("The variable's name")]
        [SerializeField] private string key;

        [Tooltip("The second operand value")]
        [SerializeField] private float value;
        [SerializeField] private bool verbose = false;

        protected override void OnStart() {
            // Try and find variable
            if(!blackboard.HasVariable(key)) 
            {
                if(verbose) 
                    Debug.LogError("Unable to find variable '"+key+"'!");
                return;
            }
            
            // Calculate and apply variable value based on operation mode
            float temp = 0.0f;
            switch(mode) 
            {
                case Mode.Set:
                    blackboard.SetValue(key, value);
                break;
                case Mode.Add:
                    temp = blackboard.GetValue(key);
                    blackboard.SetValue(key, temp+value);
                break;
                case Mode.Mult:
                    temp = blackboard.GetValue(key);
                    blackboard.SetValue(key, temp*value);
                break;
            }

            if(verbose) 
                Debug.Log("'"+key+"' value: "+blackboard.GetValue(key));
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            
            // Assume failure if no such variable exists
            if(!blackboard.HasVariable(key)) 
                return State.Failure;
  
            return State.Success;
        }
    }
}
