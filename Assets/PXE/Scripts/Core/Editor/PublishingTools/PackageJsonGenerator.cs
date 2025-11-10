using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.PublishingTools
{
    public class PackageJsonGenerator : EditorWindow
    {
        [MenuItem("PXE/Tools/Publishing Tools/Generate package.json")]
        public static void ShowWindow()
        {
            GetWindow<PackageJsonGenerator>("Generate package.json");
        }

        private string packageName = "com.yourcompany.custompackage";
        private string displayName = "Custom Package";
        private string version = "1.0.0";
        private string unityVersion = Application.unityVersion;
        private string packageJsonPath = Path.Combine(Application.dataPath, "package.json");

        private void OnGUI()
        {
            GUILayout.Label("Package Information", EditorStyles.boldLabel);

            packageName = EditorGUILayout.TextField("Package Name", packageName);
            displayName = EditorGUILayout.TextField("Display Name", displayName);
            version = EditorGUILayout.TextField("Version", version);
            unityVersion = EditorGUILayout.TextField("Unity Version", unityVersion);
            
            // Allow the user to enter an output path
            EditorGUILayout.LabelField("Output Path");
            GUILayout.BeginHorizontal();
            packageJsonPath = EditorGUILayout.TextField(packageJsonPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string fullPath = EditorUtility.OpenFolderPanel("Package File Location", Application.dataPath, "PXE");
                if (fullPath.StartsWith(Application.dataPath))
                {
                    packageJsonPath = fullPath;
                }
                else
                {
                    Debug.LogError("Please select a folder within the Assets folder!");
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate package.json"))
            {
                GeneratePackageJson();
            }
        }

        private void GeneratePackageJson()
        {
            var packageJson = new
            {
                name = packageName,
                displayName = displayName,
                version = version,
                unity = unityVersion,
                dependencies = GetDependencies()
            };

            string json = JsonConvert.SerializeObject(packageJson, Formatting.Indented);
            // string json = JsonUtility.ToJson(packageJson, true);
            // string path = Path.Combine(Application.dataPath, "package.json");
            string path = Path.Combine(packageJsonPath, "package.json");
            File.WriteAllText(path, json);

            AssetDatabase.Refresh();
            Debug.Log("package.json generated at " + path);
        }

        private static Dictionary<string, string> GetDependencies()
        {
            var dependencies = new Dictionary<string, string>();
            var listRequest = UnityEditor.PackageManager.Client.List(true);
            while (!listRequest.IsCompleted) { }

            if (listRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    dependencies.Add(package.name, package.version);
                }
            }
            else if (listRequest.Status >= UnityEditor.PackageManager.StatusCode.Failure)
            {
                Debug.LogError(listRequest.Error.message);
            }

            return dependencies;
        }
    }
}
