using UnityEngine;

namespace PXE.Core.Extensions.TransformExtensions
{
    public static class TransformExtensions
    {
        private static Vector3 ZAxis => Vector3.forward;

        public static Transform Rotate2D(this Transform transform, float by) {
            transform.Rotate(ZAxis, by, Space.World);
            return transform;
        }

        public static UnityEngine.GameObject Rotate2D(this UnityEngine.GameObject go, float by) {
            go.transform.Rotate2D(by);
            return go;
        }

        public static TComp Rotate2D<TComp>(this TComp comp, float by) where TComp : Component {
            comp.transform.Rotate2D(by);
            return comp;
        }

        public static Transform Rotate2D(this Transform transform, Vector2 at) {
            transform.Rotate2D(transform.AngleTo(at));
            return transform;
        }

        public static UnityEngine.GameObject Rotate2D(this UnityEngine.GameObject go, Vector2 at) {
            go.transform.Rotate2D(at);
            return go;
        }

        public static TComp Rotate2D<TComp>(this TComp comp, Vector2 at) where TComp : Component {
            comp.transform.Rotate2D(at);
            return comp;
        }

        private static float AngleTo(this Transform transform, Vector2 to) => AngleOf(to - transform.Position2D()) - transform.eulerAngles.z;
        private static float AngleOf(Vector2 vector) => Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        public static Vector2 Position2D(this Transform transform) => transform.position;
    }
}