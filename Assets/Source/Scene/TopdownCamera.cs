using System;
using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    public class TopdownCamera : MonoBehaviour
    {
        [SerializeField] private float smoothing = 0.15f;
        [SerializeField] private Vector3 target;

        [SerializeField] private float minDistance = 10;
        [SerializeField] private float maxDistance = 40;
        [SerializeField] private float distance = 20;

        [SerializeField] private float minAngle = 10;
        [SerializeField] private float maxAngle = 60;
        [SerializeField] private float angle = 30;

        [SerializeField] private float orbitAngle = 0;

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

        public float Angle
        {
            get => angle;
            set => angle = Mathf.Clamp(value, minAngle, maxAngle);
        }

        public float Distance
        {
            get => distance;
            set => distance = Mathf.Clamp(value, minDistance, maxDistance);
        }

        public float OrbitAngle
        {
            get => orbitAngle;
            set => orbitAngle = SanitizeAngle(value);
        }

        private void OnValidate()
        {
            Angle = Angle;
            OrbitAngle = OrbitAngle;
            Distance = Distance;
        }

        private void Awake()
        {
            SnapToTarget();
        }

        public void SnapToTarget()
        {
            m_lastTarget = m_currentTarget = target;
            m_lastAngleTarget = m_currentAngle = Angle = angle;
            m_lastDistanceTarget = m_currentDistance = Distance = distance;
            m_lastOrbitTarget = m_currentOrbit = OrbitAngle = orbitAngle;

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

        private float SanitizeAngle(float degreesAngle)
        {
            degreesAngle %= 360;
            if (degreesAngle < 0) {
                degreesAngle += 360;
            }

            return degreesAngle;
        }

        private float WrapTargetAngle(float currentAngle, float targetAngle)
        {
            // Change target angle to wrap around the shortest distance.
            // E.g. For current angle of 0 and target angle of 359 we wrap the target angle to -1,
            // which is equivalent to 359 but also much closer to the current angle of 0. 

            var diff = targetAngle - currentAngle;
            if (Mathf.Abs(diff) > 180) {
                return targetAngle - Mathf.Sign(diff) * 360;
            }

            return targetAngle;
        }

        private void Update()
        {
            // Wrapped orbit and target may not be sanitized, which is why we don't
            // modify the original orbitAngle and m_lastOrbitTarget variables
            var wrappedOrbit = WrapTargetAngle(m_currentOrbit, orbitAngle);
            var wrappedLastTarget = WrapTargetAngle(wrappedOrbit, m_lastOrbitTarget);

            // Smoothly change all properties
            m_currentAngle = MotionMath.SmoothMove(m_currentAngle, m_lastAngleTarget, angle, smoothing);
            m_currentDistance = MotionMath.SmoothMove(m_currentDistance, m_lastDistanceTarget, distance, smoothing);
            m_currentTarget = MotionMath.SmoothMove(m_currentTarget, m_lastTarget, target, smoothing);
            m_currentOrbit = MotionMath.SmoothMove(m_currentOrbit, wrappedLastTarget, wrappedOrbit, smoothing);

            m_currentOrbit = SanitizeAngle(m_currentOrbit);

            m_lastTarget = target;
            m_lastDistanceTarget = distance;
            m_lastAngleTarget = angle;
            m_lastOrbitTarget = orbitAngle;

            UpdateTransform();
        }
    }
}