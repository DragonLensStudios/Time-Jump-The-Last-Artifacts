using System.Linq;
using PXE.Core.Achievements.Managers;
using PXE.Core.Editor.Objects;
using UnityEditor;
using UnityEngine;
using Achievement = PXE.Core.Achievements.Data.Achievement;

namespace PXE.Scripts.Core.Achievements.Editor
{
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : ObjectControllerEditor
    {

        protected AchievementManager manager;
        protected int selectedAchievementIndex = 0;
        protected bool isActive = false;
        
        protected virtual void OnEnable()
        {
            if (manager == null)
            {
                manager = (AchievementManager)target;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawObjectControllerInspector();
            // Draw the default inspector
            DrawDefaultInspector();
            
            if (manager == null || manager.Manager == null || manager.Manager.AchievementList == null)
            {
                EditorGUILayout.HelpBox("Unable to access AchievementManager properties.", MessageType.Error);
                return;
            }

            for (int i = 0; i < manager.Manager.AchievementList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Achievement Label
                EditorGUILayout.LabelField("Achievement", GUILayout.Width(80));

                // Achievement Object Field without label
                Achievement achievement = (Achievement)EditorGUILayout.ObjectField(manager.Manager.AchievementList[i], typeof(Achievement), false, GUILayout.Width(285));
                manager.Manager.AchievementList[i] = achievement;

                // Reorder Up
                if (GUILayout.Button("↑", GUILayout.Width(25)) && i > 0)
                {
                    (manager.Manager.AchievementList[i], manager.Manager.AchievementList[i - 1]) = (manager.Manager.AchievementList[i - 1], manager.Manager.AchievementList[i]);
                }

                // Reorder Down
                if (GUILayout.Button("↓", GUILayout.Width(25)) && i < manager.Manager.AchievementList.Count - 1)
                {
                    (manager.Manager.AchievementList[i], manager.Manager.AchievementList[i + 1]) = (manager.Manager.AchievementList[i + 1], manager.Manager.AchievementList[i]);
                }

                // Delete
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    manager.Manager.AchievementList.RemoveAt(i);
                    continue;
                }

                EditorGUILayout.EndHorizontal();
            }

            // Add button
            if (GUILayout.Button("Add Achievement"))
            {
                manager.Manager.AchievementList.Add(null); // Add null to allow user to select an Achievement ScriptableObject
            }
            
            isActive = manager.IsActive;
            bool newIsActive = EditorGUILayout.Toggle("Is Active", isActive);
            if (newIsActive != isActive)
            {
                isActive = newIsActive;
                manager.SetObjectActive(isActive);
            }

            // Draw a button for each method
            if (GUILayout.Button("Load Achievements"))
            {
                manager.LoadAchievements();
            }
            
// Draw a dropdown for resetting specific achievement
            if (manager.Manager.AchievementList != null && manager.Manager.AchievementList.Count > 0)
            {
                // Filter out null achievements and then get their keys
                var achievementKeys = manager.Manager.AchievementList
                    .Where(achievement => achievement != null)
                    .Select(achievement => achievement.Key)
                    .ToList();

                if (achievementKeys.Count > 0) // Make sure there are valid achievements
                {
                    selectedAchievementIndex = EditorGUILayout.Popup("Reset Achievement", selectedAchievementIndex, achievementKeys.ToArray());

                    if (GUILayout.Button($"Reset {achievementKeys[selectedAchievementIndex]} Achievement"))
                    {
                        manager.ResetAchievement(achievementKeys[selectedAchievementIndex]);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No valid achievements available.", MessageType.Info);
                }
            }

            if (GUILayout.Button("Reset All Achievements"))
            {
                manager.ResetAllAchievements();
            }
        }
    }
}