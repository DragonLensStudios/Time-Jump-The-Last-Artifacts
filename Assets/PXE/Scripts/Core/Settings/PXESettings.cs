using System.Collections.Generic;
using System.IO;
using System.Linq;
using PXE.Core.Tools.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Settings
{
    //TODO: Add handling for using Asset Bundles Rather than Resources
    public static class PXESettings
    {
        private static string _projectsPath = Path.Combine("Assets","Example Games");
        public static string ProjectsPath
        {
            get => _projectsPath;
            set => _projectsPath = Path.Combine(value);
        }
// #if UNITY_EDITOR
//         public static string CurrentProjectPath
//         {
//             get => EditorPrefs.GetString($"PXU:{Application.dataPath}:CurrentProjectPath", string.Empty);
//             set
//             {
//                 EditorPrefs.SetString($"PXU:{Application.dataPath}:CurrentProjectPath", value);
//                 CurrentProjectSettings = value != string.Empty ? AssetDatabase.LoadAssetAtPath<ProjectSettingsObject>(value) : null;
//             } 
//         }
// #endif

        public static PXESettingsObject CurrentProjectSettings => Resources.FindObjectsOfTypeAll<PXESettingsObject>().FirstOrDefault();

        public static string ProjectName => CurrentProjectSettings?.CurrentProjectSettings.ProjectName;
        public static string ProjectAbbreviation => CurrentProjectSettings?.CurrentProjectSettings.ProjectAbbreviation;
        public static string Version => CurrentProjectSettings?.CurrentProjectSettings.Version;
        public static string CompanyName => CurrentProjectSettings?.CurrentProjectSettings.CompanyName;
        public static string ResourcesSubFolderFilter => CurrentProjectSettings?.CurrentProjectSettings.ResourcesFolderName;
        
        #if UNITY_EDITOR
        public static List<ProjectSettingsObject> GetAvailableProjectsObjects()
        {
            return AssetDatabase.FindAssets("t:ProjectSettingsObject", new[] {ProjectsPath})
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ProjectSettingsObject>)
                .ToList();
        }
        
        public static (string[] paths, string[] names, ProjectSettingsObject[] projectSettingsObjects) GetAvailableProjects()
        {
            var projectSettingsObjects = GetAvailableProjectsObjects().ToArray();

            var paths = projectSettingsObjects.Select(AssetDatabase.GetAssetPath).ToArray();

            var names = projectSettingsObjects.Select(p => {
                // Attempt to get project name directly
                var projectName = p.ProjectName;

                // Fallback to folder name if project name is the default
                if (projectName == ProjectSettingsObject.DefaultName) 
                {
                    projectName = Path.GetFileName(Path.GetDirectoryName(AssetDatabase.GetAssetPath(p)));
                }

                return projectName;
            }).ToArray();
            
            return (paths, names, projectSettingsObjects);
        }
        
        #endif
    }
}