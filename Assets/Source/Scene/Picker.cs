using UnityEngine;

namespace Cyens.ReInherit.Scene
{
    public static class Picker
    {
        public static bool TryPlaneIntersect(Ray ray, Vector3 planeNormal, Vector3 planePoint, out Vector3 result)
        {
            var denominator = Vector3.Dot(planeNormal, ray.direction);

            if (Mathf.Abs(denominator) > Mathf.Epsilon) {
                var t = Vector3.Dot(planePoint - ray.origin, planeNormal) / denominator;
                if (t >= 0) {
                    result = ray.origin + ray.direction * t;
                    return true;
                }
            }

            result = new Vector3(float.NaN, float.NaN, float.NaN);
            return false;
        }
    }
}