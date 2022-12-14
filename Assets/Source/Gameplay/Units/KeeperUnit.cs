using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class KeeperUnit : EmployeeUnit
    {

        public enum Task {None, MovingToElevator }

        [SerializeField] 
        private Task task;
        
        public override void SetTarget(GameObject newTarget)
        {
            base.SetTarget(newTarget);
            
            // TODO: Check if the object is an elevator door
            Elevator elevator = newTarget.GetComponent<Elevator>();
            if (elevator != null) {
                
                Debug.Log("Found an elevator");
                target = newTarget;
                m_aiPath.destination = elevator.exitPoint;
                task = Task.MovingToElevator;
            }

        }

        // TODO: List of artifacts that the keeper carries
        
        
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