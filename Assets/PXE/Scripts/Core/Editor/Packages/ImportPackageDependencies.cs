using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace PXE.Core.Editor.PublishingTools
{
    public static class ImportPackageDependencies
    {
        private static ListRequest listRequest;
        private static AddRequest addRequest;
        private static Queue<string> dependenciesQueue = new Queue<string>();

        [MenuItem("PXE/Import Package Dependencies")]
        public static void ImportDependenciesManually()
        {
            CheckForDependencies();
        }

        private static void CheckForDependencies()
        {
            EditorApplication.update -= CheckForDependencies;
            string packageJsonPath = Path.Combine(Application.dataPath, "PXE", "package.json");
            if (File.Exists(packageJsonPath))
            {
                string json = File.ReadAllText(packageJsonPath);
                var packageData = JsonUtility.FromJson<PackageData>(json);

                foreach (var dependency in packageData.dependencies)
                {
                    dependenciesQueue.Enqueue($"{dependency.Key}@{dependency.Value}");
                }

                ImportNextDependency();
            }
            else
            {
                Debug.LogWarning("package.json not found in PXE folder.");
            }
        }

        private static void ImportNextDependency()
        {
            if (dependenciesQueue.Count > 0)
            {
                string dependency = dependenciesQueue.Dequeue();
                Debug.Log($"Adding package: {dependency}");
                addRequest = Client.Add(dependency);
                EditorApplication.update += Progress;
            }
            else
            {
                Debug.Log("All dependencies have been added.");
            }
        }

        private static void Progress()
        {
            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                    Debug.Log($"Successfully added: {addRequest.Result.packageId}");
                else if (addRequest.Status >= StatusCode.Failure)
                    Debug.LogError($"Error adding package: {addRequest.Error.message}");

                EditorApplication.update -= Progress;
                ImportNextDependency();
            }
        }

        [System.Serializable]
        private class PackageData
        {
            public string name;
            public string version;
            public string displayName;
            public string description;
            public string unity;
            public Dictionary<string, string> dependencies;
        }
    }
}