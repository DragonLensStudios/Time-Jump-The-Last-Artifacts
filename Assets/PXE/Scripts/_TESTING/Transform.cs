// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;
//
// namespace PXE._TESTING
// {
//     [InitializeOnLoad]
//     public class TestAtStartup
//     {
//         static bool once = false;
//
//         static TestAtStartup()
//         {
//             // bool canDetectSimulator = ((float)PlayerSettings.defaultScreenWidth / (float)PlayerSettings.defaultScreenHeight == (float)PlayerSettings.defaultWebScreenHeight / (float)PlayerSettings.defaultWebScreenHeight);
//             // if (!canDetectSimulator) Debug.Log("Unable to detect simulator to show touch buttons, please set default desktop and web widths to different values.");
//             // if (canDetectSimulator && Screen.width == PlayerSettings.defaultScreenWidth)
//             // {
//             //     Debug.Log("Editor in Simulator Mode");
//             // } else {
//             //     Debug.Log("Editor in Game Mode");
//             // }
//
//             string modeLog = "[OnStartup] Mode detectors returned true: ";
//             #if UNITY_WEBGL
//                 modeLog += "`#if UNITY_WEBGL` is Web, ";
//             #endif
//             if (Application.isMobilePlatform) modeLog += "`Application.isMobilePlatform` is Web, ";
//             if (TouchScreenKeyboard.isSupported) modeLog += "`TouchScreenKeyboard.isSupported`, ";
//         
//             if (Debug.isDebugBuild) modeLog += "`Debug.isDebugBuild` is not Production, ";
//             #if UNITY_EDITOR
//                 modeLog += "`#if UNITY_EDITOR` or Simulator, ";
//             #endif
//             #if ENABLE_INPUT_SYSTEM
//                 modeLog += "`#if ENABLE_INPUT_SYSTEM`";
//             #endif
//             #if UNITY_INPUT_SYSTEM_ENABLE_UI
//                 modeLog += "`#if UNITY_INPUT_SYSTEM_ENABLE_UI`";
//             #endif
//
//             if (Application.isEditor) modeLog += "`Application.isEditor` or Simulator, ";
//             
//             if (Application.isPlaying) modeLog += "`Application.isPlaying`, ";
//             if (EditorApplication.isPlaying) modeLog += "`EditorApplication.isPlaying`, ";
//             if (EditorApplication.isPlayingOrWillChangePlaymode) modeLog += "`EditorApplication.isPlayingOrWillChangePlaymode`, ";
//             if (modeLog == "Mode detectors returned true: ") modeLog += "None ";
//             
//             Debug.Log(modeLog);
// /////////////////////////////////////////////////////////////////////////////
//
//             if (once) {
//                 once = false;
//                 float step = 45f;
//                 string s;
//
//                 s = "";
//                 for (float i = -720f; i < 721f; i += step)
//                 {
//                     s += TU.IsEqualish(TU.NormalizeDegrees(i), ((i + 3600f) % 360f)) ? "" : "(" + TU.NormalizeDegrees(i) + ": " + ((i + 3600f) % 360f) + "), ";
//                 }
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("1. fix TU.NormalizeDegrees(): " + s);
//
//                 s = "";
//                 s += TU.IsEqualish(TU.NormalizeDegrees(TU.DirectionToDegrees(Vector3.right)), 0f) ? "" : "(" + TU.DirectionToDegrees(Vector3.right) + ": 0), ";
//                 s += TU.IsEqualish(TU.NormalizeDegrees(TU.DirectionToDegrees(Vector3.up)), 90f) ? "" : "(" + TU.DirectionToDegrees(Vector3.up) + ": 90), ";
//                 s += TU.IsEqualish(TU.NormalizeDegrees(TU.DirectionToDegrees(Vector3.left)), 180f) ? "" : "(" + TU.DirectionToDegrees(Vector3.left) + ": 180), ";
//                 s += TU.IsEqualish(TU.NormalizeDegrees(TU.DirectionToDegrees(Vector3.down)), 270f) ? "" : "(" + TU.DirectionToDegrees(Vector3.down) + ": 270), ";
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("2. fix TU.DirectionToDegrees(): " + s);
//
//                 s = "";
//                 for (float i = 0f; i < 361f; i += step)
//                 {
//                     s += test_deg2deg(TU.DirectionToDegrees(TU.DegreesToDirection(i)), i);
//                 }
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("3 fix TU.DegreesToDirection(): " + s);
//             
//                 // Test TU.DegreesToRotation elsewhere
//                 // s = "";
//                 // for (float i = 0f; i < 361f; i += step)
//                 // {
//                 //     s += test_deg2deg(TU.RotationToDegrees(TU.DegreesToRotation(i)), i);
//                 // }
//                 // if (!string.IsNullOrWhiteSpace(s)) Debug.Log("4. fix TU.DegreesToRotation(): " + s);
//
//                 s = "";
//                 for (float i = 0f; i < 361f; i += step)
//                 {
//                     s += test_deg2deg(TU.RotationToDegrees(TU.DegreesToRotation(i)), i);
//                 }
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("5. fix TU.RotationToDegrees(): " + s);
//
//                 s = "";
//                 for (float i = 0f; i < 361f; i += step)
//                 {
//                     s += test_vec2vec(TU.RotationToDirection(TU.DegreesToRotation(i)), TU.DegreesToDirection(i));
//                 }
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("6. fix TU.RotationToDirection(): " + s);
//
//                 s = "";
//                 for (float i = 0f; i < 361f; i += step)
//                 {
//                     s += test_deg2deg(TU.RotationToDegrees(TU.DirectionToRotation(TU.DegreesToDirection(i))), i);
//                 }
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("7. fix TU.DirectionToRotation() and/or TU.RotationToDegrees() with alt_w: " + s);
//
//                 s = "";
//                 for (float i = 0f; i < 361f; i += step)
//                 {
//                     s += test_vec2vec(TU.RotationToDirection(TU.DirectionToRotation(TU.DegreesToDirection(i))), TU.DegreesToDirection(i));
//                 }
//                 if (!string.IsNullOrWhiteSpace(s)) Debug.Log("8. fix TU.RotationToDirection() with alt_w: " + s);
//
//                 // Transform transform = new Transform(); // vector3 ? = -transform.up;
//
//                 // Debug.Log("          Rotate(Vector2.up, 0): " + WAH_PlayerController.Rotate(Vector2.up, 0));
//                 // Debug.Log("          Rotate(Vector3.up, 0): " + WAH_PlayerController.Rotate(Vector3.up, 0));
//                 // Debug.Log("         TU.DegreesToRotation(-90): " + TU.DegreesToRotation(-90f));
//                 // Debug.Log("TU.DirectionToRotation(Vector3.up): " + TU.DirectionToRotation(Vector3.up));
//             }
//         }
//
//         // public static Vector2 Rotate(Vector2 v, float angle)
//         // {
//         //     return new Vector2(v.x * Mathf.Cos(angle * -Mathf.Deg2Rad) - v.y * Mathf.Sin(angle * -Mathf.Deg2Rad), v.x * Mathf.Sin(angle * -Mathf.Deg2Rad) + v.y * Mathf.Cos(angle * -Mathf.Deg2Rad));
//         // }
//
//         public static string test_deg2deg (float degrees1, float degrees2)
//         {
//             if (TU.IsEqualish(TU.NormalizeDegrees(degrees1), TU.NormalizeDegrees(degrees2)))
//             {
//                 return "";
//             }
//             else
//             {
//                 return "(" + TU.NormalizeDegrees(degrees1) + ": " + TU.NormalizeDegrees(degrees2) + "), ";
//             }
//         }
//
//         public static string test_vec2vec (Vector3 direction1, Vector3 direction2)
//         {
//             direction1 = Vector3.Normalize(direction1);
//             direction2 = Vector3.Normalize(direction2);
//             if (TU.IsEqualish(direction1.x, direction2.x) && TU.IsEqualish(direction1.y, direction2.y) && TU.IsEqualish(direction1.z, direction2.z))
//             {
//                 return "";
//             }
//             else
//             {
//                 return "(" + direction1 + ": " + direction2 + "), ";
//             }
//         }
//
//         public static string test_rot2rot (Quaternion rotation1, Quaternion rotation2)
//         {
//             Vector3 direction1 = Vector3.Normalize(TU.RotationToDirection(rotation1));
//             Vector3 direction2 = Vector3.Normalize(TU.RotationToDirection(rotation2));
//             if (TU.IsEqualish(direction1.x, direction2.x) && TU.IsEqualish(direction1.y, direction2.y) && TU.IsEqualish(direction1.z, direction2.z))
//             {
//                 return "";
//             }
//             else
//             {    
//                 return "(" + direction1 + ": " + direction2 + "), ";
//             }
//         }
//     }
// }
// #endif