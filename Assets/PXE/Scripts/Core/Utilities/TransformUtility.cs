// using PXE.Enums;
// using PXE.Interfaces;
// using UnityEngine;
//
// namespace PXE.Core.Utilities
// {
//     /// <summary>
//     ///  Represents the TransformUtility.
//     /// </summary>
//     public static class TransformUtility
//     {
//         // TODO make readonly
//         public static Quaternion right = Quaternion.identity;
//         public static Quaternion up = DegreesToRotation(90f);
//         public static Quaternion left = DegreesToRotation(180f);
//         public static Quaternion down = DegreesToRotation(270f);
//
//         public static T UpdateRotate<T>(this T hasRotateType, Vector3? Direction = null, Vector3? TargetDirection = null) where T : IRotateType
//         {
//             float degrees;
//             switch (hasRotateType.RotateType)
//             {
//                 case RotateType.None:
//                     hasRotateType.transform.rotation = Quaternion.identity;
//                     break;
//                 case RotateType.Parent:
//                     // hasRotateType.transform.localRotation = Quaternion.identity;
//                     hasRotateType.transform.rotation = hasRotateType.transform.parent.rotation;
//                     break;
//                 case RotateType.Moving:
//                     hasRotateType.transform.rotation = DirectionToRotation(Direction ?? Vector3.right);
//                     break;
//                 case RotateType.Target:
//                     hasRotateType.transform.rotation = DirectionToRotation(TargetDirection ?? Vector3.right);
//                     break;
//                 
//                 case RotateType.ParentUpDown:
//                     hasRotateType.transform.rotation = RotationToDirection(hasRotateType.transform.parent.rotation).y >= 0 ? up : down;
//                     break;
//                 case RotateType.MovingUpDown:
//                     hasRotateType.transform.rotation = (Direction ?? Vector3.right).y >= 0 ? up : down;
//                     break;
//                 case RotateType.TargetUpDown:
//                     hasRotateType.transform.rotation = (TargetDirection ?? Vector3.right).y >= 0 ? up : down;
//                     break;
//                 
//                 case RotateType.ParentLeftRight:
//                     hasRotateType.transform.rotation = RotationToDirection(hasRotateType.transform.parent.rotation).x >= 0 ? right : left;
//                     break;
//                 case RotateType.MovingLeftRight:
//                     hasRotateType.transform.rotation = (Direction ?? Vector3.right).x >= 0 ? right : left;
//                     break;
//                 case RotateType.TargetLeftRight:
//                     hasRotateType.transform.rotation = (TargetDirection ?? Vector3.right).x >= 0 ? right : left;
//                     break;
//
//                 case RotateType.Parent4Directions:
//                     degrees = RotationToDegrees(hasRotateType.transform.parent.rotation);
//                     hasRotateType.transform.rotation = DegreesToRotation(Mathf.Floor((degrees + 45f) / 90f) * 90f);
//                     break;
//                 case RotateType.Moving4Directions:
//                     degrees = DirectionToDegrees(Direction ?? Vector3.right);
//                     hasRotateType.transform.rotation = DegreesToRotation(Mathf.Floor((degrees + 45f) / 90f) * 90f);
//                     break;
//                 case RotateType.Target4Directions:
//                     degrees = DirectionToDegrees(TargetDirection ?? Vector3.right);
//                     hasRotateType.transform.rotation = DegreesToRotation(Mathf.Floor((degrees + 45f) / 90f) * 90f);
//                     break;
//
//                 case RotateType.Parent8Directions:
//                     degrees = RotationToDegrees(hasRotateType.transform.parent.rotation);
//                     hasRotateType.transform.rotation = DegreesToRotation(Mathf.Floor((degrees + 22.5f) / 45f) * 45f);
//                     break;
//                 case RotateType.Moving8Directions:
//                     degrees = DirectionToDegrees(Direction ?? Vector3.right);
//                     hasRotateType.transform.rotation = DegreesToRotation(Mathf.Floor((degrees + 22.5f) / 45f) * 45f);
//                     break;
//                 case RotateType.Target8Directions:
//                     degrees = DirectionToDegrees(TargetDirection ?? Vector3.right);
//                     hasRotateType.transform.rotation = DegreesToRotation(Mathf.Floor((degrees + 22.5f) / 45f) * 45f);
//                     break;
//             }
//             return hasRotateType;
//         }
//         
//         public static bool IsEqualish(float float1, float float2)
//         {
//             // https://docs.unity3d.com/ScriptReference/Vector3-operator_eq.html
//             return (Mathf.Abs(float2 - float1) <= Mathf.Pow(10, -5));
//             // return Mathf.Approximately(float1, float2);
//         }
//
//         public static float NormalizeDegrees(float degrees)
//         {
//             return (degrees % 360f + 360f) % 360f;
//         }
//
//         public static float DirectionToDegrees(Vector3 direction)
//         {
//             return Mathf.Atan2(direction.y * Mathf.Deg2Rad, direction.x * Mathf.Deg2Rad) * Mathf.Rad2Deg;
//             //return -Vector2.SignedAngle(direction, Vector2.right);
//         }
//
//         public static Vector3 DegreesToDirection(float degrees)
//         {
//             return new Vector3(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad), 0f);
//         }
//
//         public static Quaternion DegreesToRotation(float degrees)
//         {
//             return Quaternion.Euler(0f, 0f, degrees);
//         }
//
//         public static float RotationToDegrees(Quaternion rotation)
//         {
//             return DirectionToDegrees(rotation * Vector3.right);
//         }
//
//         public static Vector3 RotationToDirection(Quaternion rotation)
//         {
//             return rotation * Vector3.right;
//         }
//
//         public static Quaternion DirectionToRotation(Vector3 direction)
//         {
//             return Quaternion.FromToRotation(Vector3.right, direction);
//         }
//     }
// }
