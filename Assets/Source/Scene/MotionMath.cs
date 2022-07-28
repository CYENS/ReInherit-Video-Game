using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    public static class MotionMath
    {
        public const float Epsilon = 0.0001f;
        
        public static float SmoothMove(float position, float lastTarget, float currentTarget, float smoothing)
        {
            if (smoothing <= Epsilon) {
                return currentTarget;
            }

            var smoothFactor = 1 / smoothing * Time.deltaTime;

            if (smoothFactor <= Epsilon) {
                return position;
            }

            var v = (currentTarget - lastTarget) / smoothFactor;
            var f = position - lastTarget + v;

            return currentTarget - v + f * Mathf.Exp(-smoothFactor);
        }

        public static Vector2 SmoothMove(Vector2 position, Vector2 lastTarget, Vector2 currentTarget, float smoothing)
        {
            if (smoothing <= Epsilon) {
                return currentTarget;
            }

            var smoothFactor = 1 / smoothing * Time.deltaTime;

            if (smoothFactor <= Epsilon) {
                return position;
            }

            var v = (currentTarget - lastTarget) / smoothFactor;
            var f = position - lastTarget + v;

            return currentTarget - v + f * Mathf.Exp(-smoothFactor);
        }

        public static Vector3 SmoothMove(Vector3 position, Vector3 lastTarget, Vector3 currentTarget, float smoothing)
        {
            if (smoothing <= Epsilon) {
                return currentTarget;
            }

            var smoothFactor = 1 / smoothing * Time.deltaTime;

            if (smoothFactor <= Epsilon) {
                return position;
            }

            var v = (currentTarget - lastTarget) / smoothFactor;
            var f = position - lastTarget + v;

            return currentTarget - v + f * Mathf.Exp(-smoothFactor);
        }
    }
}