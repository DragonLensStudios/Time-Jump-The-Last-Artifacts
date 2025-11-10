using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Scripts.Tools.Editor_Utility.PIxel_Rename_Tool.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RenameToolPreset", menuName = "PXE/Tools/RenameTool/Create Preset", order = 1)]
    public class RenameToolPresetObject : ScriptableObject
    {
        public RenamingMode renamingMode;
        public string prefix;
        public string suffix;
        public string newName;
        public IncrementalValueType incrementalType;
        public string renameRegexPattern;
        public string placementRegexPattern;
        public int startNumber;
        public int renameSelectedCustomPatternIndex;
        public string renameCustomPattern;
        public PlacementStrategy placementStrategy;
        // Additional fields to match the full editor functionality
        public string regexPrefix;
        public string regexSuffix;
        public CasingType casingType;
        public string incrementalCustomPattern;
        public int incrementalSelectedCustomPatternIndex = 0;
        public int selectedDateFormatIndex = 0;
        public int selectedTimeFormatIndex = 0;
        public string selectedDateFormat = "yyyy-MM-dd"; // ISO 8601
        public string selectedTimeFormat = "HH:mm"; // 24-hour format
    }
}