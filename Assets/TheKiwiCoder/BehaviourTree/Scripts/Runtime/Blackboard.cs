using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard {
        public Vector3 moveToPosition;
        public GameObject garbage;
        public List<Variable> variables;
    }

    // Variables that can be used by the agent at runtime
    [System.Serializable]
    public struct Variable
    {
        public string name;
        public float value;
    }
}