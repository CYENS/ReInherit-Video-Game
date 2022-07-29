using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    public class TopdownCamera : MonoBehaviour
    {
        [SerializeField] private float smoothing = 0.15f;
        [SerializeField] private Vector3 target;

        [SerializeField, Range(0, 360)] private float orbitAngle = 0;

        [SerializeField, Range(10, 40)] private float distance = 20;

        [SerializeField, Range(15, 60)] private float angle = 30;

        private Vector3 m_currentTarget;
        private Vector3 m_lastTarget;

        private float m_lastDistanceTarget;
        private float m_lastAngleTarget;
        private float m_lastOrbitTarget;

        private float m_currentAngle;
        private float m_currentDistance;
        private float m_currentOrbit;

        public Vector3 Target
        {
            get => target;
            set => target = value;
        }

        public float Distance
        {
            get => distance;
            set => distance = Mathf.Max(10, value);
        }

        public float OrbitAngle
        {
            get => orbitAngle;
            set {
                orbitAngle = value % 360;
                if (orbitAngle < 0) {
                    orbitAngle += 360;
                }
            }
        }

        private void Awake()
        {
            m_currentAngle = angle;
            m_currentDistance = distance;
            m_currentTarget = target;
        }

        public void SnapToTarget()
        {
            m_lastTarget = m_currentTarget = target;
            m_lastAngleTarget = m_currentAngle = angle;
            m_lastDistanceTarget = m_currentDistance = distance;
            m_lastOrbitTarget = m_currentOrbit = orbitAngle;

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            var orbit = Quaternion.AngleAxis(m_currentOrbit, Vector3.up);
            var forward = orbit * Vector3.forward;

            var radians = Mathf.Deg2Rad * m_currentAngle;
            var height = Mathf.Sin(radians) * m_currentDistance;
            var distanceXZ = Mathf.Cos(radians) * m_currentDistance;
            var newPosition = m_currentTarget - forward * distanceXZ;
            newPosition.y += height;
            
            transform.position = newPosition;
            transform.LookAt(m_currentTarget);
        }

        private void Update()
        {
            // Change orbit angle to wrap around the shortest distance.
            // E.g. For current angle of 0 and orbit angle of 359 we use a
            //      wrapOrbitAngle of -1, which is equivalent to 359 but also
            //      much closer to 0. 
            var wrapOrbitAngle = orbitAngle;
            var orbitDiff = orbitAngle - m_currentOrbit;
            if (Mathf.Abs(orbitDiff) > 180) {
                wrapOrbitAngle = orbitAngle - Mathf.Sign(orbitDiff) * 360;
            }

            // Smoothly change all properties
            m_currentAngle = MotionMath.SmoothMove(m_currentAngle, m_lastAngleTarget, angle, smoothing);
            m_currentDistance = MotionMath.SmoothMove(m_currentDistance, m_lastDistanceTarget, distance, smoothing);
            m_currentTarget = MotionMath.SmoothMove(m_currentTarget, m_lastTarget, target, smoothing);
            m_currentOrbit = MotionMath.SmoothMove(m_currentOrbit, m_lastOrbitTarget, wrapOrbitAngle, smoothing);

            m_lastTarget = target;
            m_lastDistanceTarget = distance;
            m_lastAngleTarget = angle;
            m_lastOrbitTarget = wrapOrbitAngle;

            UpdateTransform();
        }
    }
}