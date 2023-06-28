using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Contains utilities for basic functions used by game agents (characters).
    /// 
    /// </summary>
    public class BaseAgent : MonoBehaviour
    {

        private NavMeshAgent m_navAgent;


        [System.Serializable]
        public struct Task 
        {
            public enum Type { 
                Wait = 0,
                MoveTo = 1,
                Look = 2,
                Activate = 3,
                Deactivate = 4,
                Message = 5
            }
            public Type type;

            // Values
            public float threshold;
            public float speed;
            public float delay;
            public Vector3 point;
            public string message;

            // References
            public Transform transform;
            public GameObject gameObject;

        }

        protected Queue<Task> m_remainingTasks;

        [Tooltip("The current task to execute")]
        protected Task m_task;

        [Tooltip("Whether the agent is ready to execute the very next task")]
        protected bool m_nextReady;
        

        public bool isIdle => (m_remainingTasks == null || m_remainingTasks.Count == 0) && m_nextReady;
        public bool isBusy => !isIdle;

        protected float m_baseSpeed;


        [Header("Task Variables")]

        [SerializeField]
        private float m_timer;


        protected void Start() 
        {
            m_navAgent = GetComponent<NavMeshAgent>();
            m_remainingTasks = new Queue<Task>();
            m_timer = 0.0f;
            m_baseSpeed = m_navAgent.speed;
            m_nextReady = true;
        }

        public void AddWaitTask( float delay ) 
        {
            Task newTask = new Task();
            newTask.type = Task.Type.Wait;
            newTask.delay = delay;
            m_remainingTasks.Enqueue(newTask);
        }
        public void AddMoveTask( Vector3 point, float speed = 1.0f, float threshold = 0.1f ) 
        {
            Task newTask = new Task();
            newTask.type = Task.Type.MoveTo;
            newTask.point = point;
            newTask.speed = speed;
            newTask.threshold = threshold;
            m_remainingTasks.Enqueue(newTask);
        }

        public void AddMoveTask( Transform target, float speed = 1.0f, float threshold = 0.1f )
        {
            AddMoveTask( target.position, speed, threshold );
        }

        public void AddLookTask( Transform target, float speed = 10.0f, float threshold = 0.1f )
        {
            Task newTask = new Task();
            newTask.type = Task.Type.Look;
            newTask.speed = speed;
            newTask.transform = target;
            newTask.threshold = threshold;
            m_remainingTasks.Enqueue(newTask);
        }

        public void AddActivateTask( GameObject target )
        {
            Task newTask = new Task();
            newTask.type = Task.Type.Activate;
            newTask.gameObject = target;
            m_remainingTasks.Enqueue(newTask); 
        }

        public void AddDeactivateTask( GameObject target )
        {
            Task newTask = new Task();
            newTask.type = Task.Type.Deactivate;
            newTask.gameObject = target;
            m_remainingTasks.Enqueue(newTask); 
        }
        public void AddMessageTask( GameObject target, string message )
        {
            Task newTask = new Task();
            newTask.type = Task.Type.Message;
            newTask.gameObject = target;
            newTask.message = message;
            m_remainingTasks.Enqueue(newTask); 
        }

        protected bool ExecuteWaitTask( Task task )
        {
            m_timer += Time.deltaTime;
            return m_timer >= task.delay;
        }

        protected bool ExecuteMoveTask( Task task )
        {
            m_navAgent.isStopped = false;

            // Code to ensure we don't call set destination too many times
            Vector3 oldDestination = m_navAgent.destination;
            Vector3 destination = task.point;
            float difference = Vector3.Distance(oldDestination,destination);
            if( difference > float.Epsilon )
            {
                m_navAgent.SetDestination(task.point);
            }

            // Wait for the path to calculate
            if( m_navAgent.pathPending )
            {
                return false;
            }

            // Set agent movement speed
            m_navAgent.speed = task.speed * m_baseSpeed;

            // Check to see if we have reached our destination
            float distance = m_navAgent.remainingDistance;
            if( distance <= task.threshold )
            {
                m_navAgent.isStopped = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper function that makes the character turn towards a certain direction
        /// </summary>
        /// <param name="target"></param>
        /// <param name="turnSpeed"></param>
        protected void LookAt(Vector3 target, float turnSpeed = 10.0f)
        {
            Vector3 myPos = transform.position;
            Vector3 toTarget = target - myPos;
            toTarget.y = 0;

            transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.LookRotation(toTarget),
                    Time.deltaTime * turnSpeed
                );
        }

        protected bool ExecuteLookTask( Task task )
        {
            Transform target = task.transform;
            Vector3 targetPos = target.position;

            LookAt( targetPos, task.speed );

            Vector3 forward = transform.forward;

            Vector3 myPos = transform.position;
            Vector3 toTarget = targetPos - myPos;
            toTarget.y = 0;

            float difference = Vector3.Angle(forward,toTarget);
            return difference <= task.threshold;
        }

        protected bool ExecuteActivateTask( Task task ) 
        {
            GameObject target = task.gameObject;
            target.SetActive(true);
            return true;
        }

        protected bool ExecuteDeactivateTask( Task task ) 
        {
            GameObject target = task.gameObject;
            target.SetActive(false);
            return true;
        }

        protected bool ExecuteMessageTask( Task task ) 
        {
            GameObject target = task.gameObject;
            target.SendMessage(task.message);
            return true;
        }

        protected bool ExecuteTask( Task task )
        {
            switch( task.type )
            {
                case Task.Type.Wait: return ExecuteWaitTask(task);
                case Task.Type.MoveTo: return ExecuteMoveTask(task);
                case Task.Type.Look: return ExecuteLookTask(task);
                case Task.Type.Activate: return ExecuteActivateTask(task);
                case Task.Type.Deactivate: return ExecuteDeactivateTask(task);
                case Task.Type.Message: return ExecuteMessageTask(task);
            }
            return false;   
        }


        protected void UpdateTasks()
        {
            // Try to get next task
            if( m_nextReady )
            {
                if(m_remainingTasks.Count == 0)
                {
                    // No more tasks to execute
                    return;
                }
                
                m_task = m_remainingTasks.Dequeue();
                m_nextReady = false;
                m_timer = 0.0f;
            }

            bool done = ExecuteTask(m_task);
            if( done )
            {
                m_nextReady = true;
            }
        }


        protected virtual void Update() 
        {
            UpdateTasks();
        }
    }
}
