using PXE.Core.Actor;
using PXE.Core.Enums;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Objects
{
    [CustomEditor(typeof(PatrolObjectController), true)]
    public class PatrolObjectControllerEditor : ObjectControllerEditor
    {
        protected int selectedIndex = -1; // Store the currently selected index
        protected bool showPatrolPoints = true;  // Initial state of the collapsible section
        protected bool isPatrolPointSelected = false;
        protected bool isActive;

        private void OnSceneGUI()
        {
            PatrolObjectController patrolObjectController = (PatrolObjectController)target;

            Handles.color = Color.yellow; // Color for the patrol path

            for (int i = 0; i < patrolObjectController.PatrolPoints.Count; i++)
            {
                Handles.DrawSolidDisc(patrolObjectController.PatrolPoints[i], Vector3.back, 0.2f);

                // Depending on the mode, draw the gizmo path differently
                switch (patrolObjectController.CurrentPatrolMode)
                {
                    case PatrolMode.Loop:
                        if (i < patrolObjectController.PatrolPoints.Count - 1)
                            Handles.DrawLine(patrolObjectController.PatrolPoints[i], patrolObjectController.PatrolPoints[i + 1]);
                        else
                            Handles.DrawLine(patrolObjectController.PatrolPoints[i], patrolObjectController.PatrolPoints[0]);
                        break;
                    
                    case PatrolMode.OneWay:
                        if (i < patrolObjectController.PatrolPoints.Count - 1)
                            Handles.DrawLine(patrolObjectController.PatrolPoints[i], patrolObjectController.PatrolPoints[i + 1]);
                        break;

                    case PatrolMode.PingPong:
                        if (i < patrolObjectController.PatrolPoints.Count - 1)
                            Handles.DrawLine(patrolObjectController.PatrolPoints[i], patrolObjectController.PatrolPoints[i + 1]);
                        break;

                    case PatrolMode.Dynamic:
                        // You'll need extra logic for this mode if it differs from the loop in gizmo representation. For now, it'll just loop.
                        if (i < patrolObjectController.PatrolPoints.Count - 1)
                            Handles.DrawLine(patrolObjectController.PatrolPoints[i], patrolObjectController.PatrolPoints[i + 1]);
                        else
                            Handles.DrawLine(patrolObjectController.PatrolPoints[i], patrolObjectController.PatrolPoints[0]);
                        break;
                }

                EditorGUI.BeginChangeCheck();
                Vector2 newTargetPosition = Handles.PositionHandle(patrolObjectController.PatrolPoints[i], Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(patrolObjectController, "Change Patrol Point Position");
                    patrolObjectController.PatrolPoints[i] = newTargetPosition;
                }

                if (selectedIndex != i) continue;
                // Disable Transform tool
                UnityEditor.Tools.current = Tool.None;
                isPatrolPointSelected = true;
            }
            
            // Re-enable Transform tool if no patrol point is selected
            if (!isPatrolPointSelected)
            {
                UnityEditor.Tools.current = Tool.Move;
            }

            isPatrolPointSelected = false;
        }

        public override void OnInspectorGUI()
        {
            DrawObjectControllerInspector();
            DrawDefaultInspector();

            PatrolObjectController patrolObjectController = (PatrolObjectController)target;

            // Start the collapsible section for Patrol Points
            showPatrolPoints = EditorGUILayout.Foldout(showPatrolPoints, "Patrol Points");
            if (showPatrolPoints)
            {
                // Draw the Patrol Points list with buttons for selection
                for (int i = 0; i < patrolObjectController.PatrolPoints.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Smaller label for patrol point
                    EditorGUILayout.LabelField($"P{i + 1}", GUILayout.Width(40));

                    // Larger Vector2 field
                    patrolObjectController.PatrolPoints[i] = EditorGUILayout.Vector2Field("", patrolObjectController.PatrolPoints[i], GUILayout.Height(20));

                    // Button to focus on the patrol point in the Scene view
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        selectedIndex = i;
                        SceneView.lastActiveSceneView.pivot = patrolObjectController.PatrolPoints[i];
                        SceneView.lastActiveSceneView.Repaint();
                    }

                    // Button (with "X" label) to delete the patrol point
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        Undo.RecordObject(patrolObjectController, "Delete Patrol Point");
                        patrolObjectController.PatrolPoints.RemoveAt(i);
                        return; // Exit the method early to prevent potential out-of-range errors
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Patrol Point"))
                {
                    Undo.RecordObject(patrolObjectController, "Add Patrol Point");
                    Vector2 newPatrolPoint = patrolObjectController.PatrolPoints.Count > 0 ? patrolObjectController.PatrolPoints[patrolObjectController.PatrolPoints.Count - 1] : patrolObjectController.transform.position;
                    newPatrolPoint += Vector2.down; // Move the point 1 unit down on the y-axis
                    patrolObjectController.PatrolPoints.Add(newPatrolPoint);
                }

                if (patrolObjectController.PatrolPoints.Count > 0 && GUILayout.Button("Remove Last Patrol Point"))
                {
                    Undo.RecordObject(patrolObjectController, "Remove Patrol Point");
                    patrolObjectController.PatrolPoints.RemoveAt(patrolObjectController.PatrolPoints.Count - 1);
                }
            }
        }
    }
}