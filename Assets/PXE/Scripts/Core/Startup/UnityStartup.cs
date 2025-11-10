using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
#endif

namespace PXE.Core.Startup
{
    public class UnityStartup
    {
        static UnityStartup()
        {

        
        }
        
        // [InitializeOnLoadMethod]
        // public static void OnEditorLoaded()
        // {
//             Debug.Log("Editor loaded");
// #if UNITY_EDITOR
//             // Settings.CurrentProjectSettings = AssetDatabase.LoadAssetAtPath<PXESettingsObject>(Settings.CurrentProjectPath);
//             if(PXESettings.CurrentProjectSettings == null)
//             {
//                 var projects = PXESettings.GetAvailableProjects().projectSettingsObjects;
//                 if(projects.Length > 0)
//                 {
//                     PXESettings.ProjectsPath = AssetDatabase.GetAssetPath(projects[0]);
//                 }
//                 else
//                 {
//                     ProjectSettingsEditor.ShowWindow();
//                 }
//             }
//             Debug.Log("Loaded Current Project: " + PXESettings.CurrentProjectSettings?.CurrentProjectSettings?.ProjectName);
//             //TODO: Replace on scene loaded with correct runtime event handler below
//             SceneManager.sceneLoaded += OnEditorSceneLoaded;
//             if(SceneManager.GetActiveScene() != null)
//             {
//                 OnEditorSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
//             }
// #endif
            
            
        // }

        public static void OnEditorSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            // ObjectController.UpdateAllIdentities();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            // Debug.Log("Before SplashScreen is shown and before the first scene is loaded.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            // Debug.Log("First scene loading: Before Awake is called.");
            // ObjectController.UpdateAllIdentities();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            // Debug.Log("First scene loaded: After Awake is called.");
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeInitialized()
        {
            // Debug.Log("Runtime initialized: First scene loaded: After Awake is called.");
        }
    }
}
