using System.IO;
using System.Linq;
using PXE.Core.Objects;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Tools.Editor
{
//TODO: For any lints that point to broken objects in the hiearchy use the following log format:Log(string,ObjectInHiearchey) when doubleclicking the log it should select the object in the hiearchy
//TODO: Add linting support to check to make sure virtual is used on all methods that are intended to be overridden exclude static and non public properties with backing fields.
//TODO: Add linting support to check that [field: SerializeField] is used on all properties that are intended to be serialized.
    public class LintToolEditor
    {
        public const string LintIgnoreComment = "///Lint:Ignore";
        public static string[] ScanFolders = { "Assets/_Game", "Assets/Example Games" };

        [MenuItem("PXE/Tools/Lint/All Scripts")]
        public static void LintAllScripts()
        {
            foreach (var folder in ScanFolders)
            {
                var csFiles = AssetDatabase.FindAssets("t:TextAsset", new[] { folder });
                foreach (var guid in csFiles)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.EndsWith(".cs")) continue;
                    LintScript(path);
                }
            }
        }
    
        public static void LintScript(string path)
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script == null) return;
            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Contains("async") || line.Contains("await")) ///Lint:Ignore
                {
                    if (lines.Take(i + 1).Any(l => l.Contains(LintIgnoreComment))) continue;
                    string message = $"{script.name}: Contains async/await at line {i + 1}. This is not allowed in Unity scripts since it breaks web export.";
                    string filePath = path.Replace(Application.dataPath, "Assets");
                    Debug.LogWarning($"{message}\n{filePath}({i + 1},0)");
                }
            }
        }
        
        [MenuItem("PXE/Tools/Lint/All GameObjects")]
        public static void LintAllGameObjects()
        {
            ObjectController.UpdateAllIdentities();
        }
        
    }
    
    
    // LogWithFileLink(message, filePath, i + 1, script);

    // string message = $"{script.name}: Contains async/await at line {i + 1}. This is not allowed in Unity scripts since it breaks web export.";
    // string filePath = path.Replace(Application.dataPath, "Assets");
    // Debug.LogError($"{message}\n{filePath}({i + 1},0)");
    //Working: Character.ActorController:OnDie () (at Assets/_Game/Scripts/Character/ActorController.cs:250)
    
    
    // private static void LogWithFileLink(string message, string filePath, int lineNumber, MonoScript script)
    // {
    //     throw new LintWarningException(message, filePath, lineNumber);
    //     // var exception = new LintWarningException(message, filePath, lineNumber);
    //     // Debug.LogException(exception);
    //     // Debug.LogException(exception,script);
    // }
    
    // public class LintWarningException : Exception
    // {
    //     public string FilePath { get; }
    //     public int LineNumber { get; }
    //
    //     public LintWarningException(string message, string filePath, int lineNumber) : base(message)
    //     {
    //         FilePath = filePath;
    //         LineNumber = lineNumber;
    //     }
    //
    //     public override string StackTrace
    //     {
    //         get
    //         {
    //             return $"{FilePath}:{LineNumber}\n" + base.StackTrace;
    //         }
    //     }
    // }
}