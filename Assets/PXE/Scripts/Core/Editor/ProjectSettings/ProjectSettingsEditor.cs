using System;
using System.IO;
using PXE.Core.Tools.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.ProjectSettings
{
    public class ProjectSettingsEditor : EditorWindow
    {
        // protected string[] availableProjectsNames = Array.Empty<string>();
        protected (string[] paths, string[] names, ProjectSettingsObject[] projectSettingsObjects) availableProjects = (Array.Empty<string>(),Array.Empty<string>(), Array.Empty<ProjectSettingsObject>());
        
        [MenuItem("PXE/Project Settings", priority = 0 )]
        public static void ShowWindow()
        {
            GetWindow<ProjectSettingsEditor>(false, "Pixel Engine Project Settings", true);
        }
        
        public virtual void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Projects Path: " + Settings.PXESettings.ProjectsPath);
            if (GUILayout.Button("Select Projects Path"))
            {
                Settings.PXESettings.ProjectsPath = EditorUtility.OpenFolderPanel("Select Directory", "Assets", "");
                Settings.PXESettings.ProjectsPath = Settings.PXESettings.ProjectsPath.Replace(Application.dataPath, "Assets");
                availableProjects = Settings.PXESettings.GetAvailableProjects();
            }
            GUILayout.EndHorizontal();

            //TODO: Add Create New Project functionality
            if (GUILayout.Button("Create New Project..."))
            {
                //TODO: On Create New Project default all unity settings to the current project settings defualt company name Application.companyName
                CreateAndSaveNewProjectSettings();
            }
            
            GUILayout.BeginHorizontal();
            if (Settings.PXESettings.CurrentProjectSettings != null)
            {
                GUILayout.Label($"Current Project: {Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ProjectName}");
                if (GUILayout.Button("Select Current Project"))
                {
                    Settings.PXESettings.ProjectsPath = string.Empty;
                    availableProjects = Settings.PXESettings.GetAvailableProjects();
                }
            }
            else
            {
                var availableProjectsSelectedIndex = EditorGUILayout.Popup(-1, availableProjects.names);
                if (availableProjectsSelectedIndex != -1)
                {
                    Settings.PXESettings.ProjectsPath = AssetDatabase.GetAssetPath(availableProjects.projectSettingsObjects[availableProjectsSelectedIndex]);
                }
            }
            
            GUILayout.EndHorizontal();

            if (Settings.PXESettings.CurrentProjectSettings != null)
            {
                Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ProjectName = EditorGUILayout.TextField("Project Name", Settings.PXESettings.ProjectName);
                Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ProjectAbbreviation = EditorGUILayout.TextField("Project Abbreviation", Settings.PXESettings.ProjectAbbreviation);
                Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.Version = EditorGUILayout.TextField("Version", Settings.PXESettings.Version);
                Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.CompanyName = EditorGUILayout.TextField("Company Name", Settings.PXESettings.CompanyName);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Resources Sub Folder Filter: " + Settings.PXESettings.ResourcesSubFolderFilter);
                if (GUILayout.Button("Select Resources Sub Folder"))
                {
                    var basePath = Path.GetFullPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(Settings.PXESettings.CurrentProjectSettings)));
                    var rootPath = Path.Combine(basePath, "Resources");
                    //GetFullPath is used to ensure that the path is in the correct format for the following replace
                    var selectedPath = Path.GetFullPath(EditorUtility.OpenFolderPanel("Select Directory", rootPath, ""));
                    Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ResourcesFolderName = selectedPath.Replace(rootPath + Path.DirectorySeparatorChar, "");
                }
                GUILayout.EndHorizontal();
            }

            if(GUILayout.Button("Save Current Project Settings"))
            {
                SaveCurrentProjectSettings();
            }
            GUILayout.EndVertical();
        }
        
        public virtual void CreateAndSaveNewProjectSettings()
        {
            ProjectSettingsObject projectSettingsAsset = CreateInstance<ProjectSettingsObject>();
            string path = EditorUtility.SaveFilePanelInProject("Save New Project Settings", "Project Settings", "asset", "Please enter a file name to save the new project settings.");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(projectSettingsAsset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Settings.PXESettings.ProjectsPath = AssetDatabase.GetAssetPath(projectSettingsAsset);
            }
        }
        
        public virtual void SaveCurrentProjectSettings()
        {
            if(Settings.PXESettings.CurrentProjectSettings == null)
            {
                Debug.LogError("Current Project Settings is null. Please select a project or create a new one.");
                return;
            }
            Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ProjectName = Settings.PXESettings.ProjectName;
            Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ProjectAbbreviation = Settings.PXESettings.ProjectAbbreviation;
            Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.Version = Settings.PXESettings.Version;
            Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.CompanyName = Settings.PXESettings.CompanyName;
            Settings.PXESettings.CurrentProjectSettings.CurrentProjectSettings.ResourcesFolderName = Settings.PXESettings.ResourcesSubFolderFilter;
            
            EditorUtility.SetDirty(Settings.PXESettings.CurrentProjectSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //TODO: Try to get exporting to pull the data from the settings object
            
        }
    }
}