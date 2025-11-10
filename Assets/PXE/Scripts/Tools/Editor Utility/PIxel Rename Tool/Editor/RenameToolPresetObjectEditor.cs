using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Scripts.Tools.Editor_Utility.PIxel_Rename_Tool.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Tools.Editor_Utility.PIxel_Rename_Tool.Editor
{
    [CustomEditor(typeof(RenameToolPresetObject))]
    public class RenameToolPresetObjectEditor : UnityEditor.Editor
    {
    
        // Define a list of common Regex templates
        public KeyValuePair<string, string>[] regexTemplates = new[]
        {
            new KeyValuePair<string, string>("None", ""), // Default option with an empty string
            new KeyValuePair<string, string>("Digits Only", @"\d+"),
            new KeyValuePair<string, string>("Alphabets Only", @"[A-Za-z]+"),
            new KeyValuePair<string, string>("Alphanumeric", @"[A-Za-z0-9]+"),
            new KeyValuePair<string, string>("Valid Email", @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}"),
            new KeyValuePair<string, string>("URL", @"https?://(?:www\.)?[a-zA-Z0-9./-]+"),
            new KeyValuePair<string, string>("File Path (Linux/Unix)", @"/[^/ ]+/?.+"),
            new KeyValuePair<string, string>("File Path (Windows)", @"[a-zA-Z]:\\[^\\/:*?""><|\r\n]+"),
            new KeyValuePair<string, string>("Date (YYYY-MM-DD)", @"\d{4}-\d{2}-\d{2}"),
            new KeyValuePair<string, string>("Time (HH:MM 24hr)", @"(?:[01]\d|2[0-3]):[0-5]\d"),
            new KeyValuePair<string, string>("IP Address", @"\b(?:\d{1,3}\.){3}\d{1,3}\b"),
        };
    
        // Preset patterns for user selection
        public string[] customPatternPresets = {
            "None", // Default option with an empty string
            "{baseName}_{count}",
            "{projectId}_{version}_{platform}_{baseName}_{date}",
            "{baseName}_{date}_{time}",
            "Asset_{count}_{platform}",
            "{year}_{month}_{day}_{baseName}",
            "Version_{version}_{baseName}",
            "{baseName}_Final_{version}",
            "Preview_{baseName}_{count}",
            "{baseName}_Backup_{dateTime}",
            "{baseName}_{guid}",
            "{platform}_Build_{buildVersion}_{baseName}",
            "{baseName}_{projectId}_{version}_{platform}_Final",
            "{companyName}_{productName}_{baseName}",
            "{companyName}_{baseName}_{version}",
            "Resolution_{resolution}_{baseName}",
            "{productName}_Feature_{baseName}_{date}",
            "{platform}_{productName}_{resolution}",
            "{companyName}_{productName}_{platform}_{date}",
            "{baseName}_{resolution}_{platform}_Build",
            // Feel free to add more based on the available placeholders
        };
    
        // Lists of common date and time formats
        public string[] dateFormatOptions = new[]
        {
            "None", // No date format
            "yyyy-MM-dd", // ISO 8601
            "MM/dd/yyyy",
            "dd/MM/yyyy",
            "yyyy/MM/dd",
            "dd-MM-yyyy",
            "MM-dd-yyyy",
            "yyyyMMdd", // Basic ISO date
            "dd MMM yyyy", // e.g., 01 Jan 2020
            "MMMM dd, yyyy", // e.g., January 01, 2020
            "dd MMMM yyyy", // e.g., 01 January 2020
            "yyyy MMM dd", // e.g., 2020 Jan 01
            "yyyy MMMM dd", // e.g., 2020 January 01
            "dd-MM-yy", // Short year format
            "MM-dd-yy",
            "yy/MM/dd",
            "dd/MM/yy",
            // Add more as needed
        };
    
        public string[] timeFormatOptions = new[]
        {
            "None", // No time format
            "HH:mm", // 24-hour format
            "hh:mm tt", // 12-hour format with AM/PM
            "HH:mm:ss",
            "hh:mm:ss tt",
            "HHmm", // Basic 24-hour format without separator
            "hhmm tt", // Basic 12-hour format without separator
            "HH:mm:ss.fff", // With milliseconds
            "hh:mm:ss.fff tt", // 12-hour format with milliseconds
            "HHmmss", // Compact 24-hour format
            "hhmmsstt", // Compact 12-hour format
            "HH:mm:ss:fff", // Alternative millisecond separator
            "hh:mm:ss:fff tt",
            // Add more as needed
        };

        public override void OnInspectorGUI()
        {
            RenameToolPresetObject preset = (RenameToolPresetObject)target;
        
            EditorGUILayout.Space();

            preset.renamingMode = (RenamingMode)EditorGUILayout.EnumPopup("Renaming Mode", preset.renamingMode);

            switch (preset.renamingMode)
            {
                case RenamingMode.Standard:
                    DrawStandardFields(preset);
                    break;
                case RenamingMode.Pattern:
                    DrawPatternFields(preset);
                    break;
                case RenamingMode.Regex:
                    DrawRegexFields(preset);
                    break;
            }

            EditorGUILayout.Space();

            preset.incrementalType = (IncrementalValueType)EditorGUILayout.EnumPopup("Incremental Type", preset.incrementalType);
            // Fields based on Incremental Type
            if (preset.incrementalType != IncrementalValueType.None && preset.incrementalType != IncrementalValueType.DateTimeStamp 
                                                                    && preset.incrementalType != IncrementalValueType.UniqueID)
            {

                if (preset.incrementalType == IncrementalValueType.CustomPattern)
                {
                    DrawIncrementalPatternHelperUI(preset);
                }
                else
                {
                    preset.startNumber = EditorGUILayout.IntField("Start Number", preset.startNumber);
                }
            }

            EditorGUILayout.Space();

            if (preset.incrementalType != IncrementalValueType.None)
            {
                preset.placementStrategy = (PlacementStrategy)EditorGUILayout.EnumPopup("Placement Strategy", preset.placementStrategy);
                if (preset.placementStrategy == PlacementStrategy.RegexPattern)
                {
                    DrawRegexHelperUI("Placement Regex Template", ref preset.placementRegexPattern);
                    preset.placementRegexPattern = EditorGUILayout.TextField("Placement Regex Pattern", preset.placementRegexPattern);
                }
            }

            EditorGUILayout.Space();

            preset.casingType = (CasingType)EditorGUILayout.EnumPopup("Casing Type", preset.casingType);
        
            EditorGUILayout.Space();
            DrawDateTimeFormatSelection(preset);


            if (!GUI.changed) return;
            EditorUtility.SetDirty(preset);
        }
    
        public virtual void DrawIncrementalPatternHelperUI(RenameToolPresetObject preset)
        {
            // Temporary index to hold the dropdown selection
            int newIndex = EditorGUILayout.Popup("Incremental Custom Pattern", preset.incrementalSelectedCustomPatternIndex, customPatternPresets);
            if (newIndex != preset.incrementalSelectedCustomPatternIndex)
            {
                preset.incrementalSelectedCustomPatternIndex = newIndex;
                // Update the customPattern with the selected preset for preview, but don't apply it yet
                if (preset.incrementalSelectedCustomPatternIndex >= 1)
                {
                    preset.incrementalCustomPattern = customPatternPresets[preset.incrementalSelectedCustomPatternIndex];
                }
            }
            preset.incrementalCustomPattern = EditorGUILayout.TextField("Custom Pattern", preset.incrementalCustomPattern);
        }

        public virtual void DrawStandardFields(RenameToolPresetObject preset)
        {
            preset.prefix = EditorGUILayout.TextField("Prefix", preset.prefix);
            preset.suffix = EditorGUILayout.TextField("Suffix", preset.suffix);
            preset.newName = EditorGUILayout.TextField("New Name", preset.newName);
        }

        public virtual void DrawPatternFields(RenameToolPresetObject preset)
        {
            // Temporary index to hold the dropdown selection
            int newIndex = EditorGUILayout.Popup("Select Pattern Preset", preset.renameSelectedCustomPatternIndex, customPatternPresets);
            if (newIndex != preset.renameSelectedCustomPatternIndex)
            {
                preset.renameSelectedCustomPatternIndex = newIndex;
                // Update the customPattern with the selected preset for preview, but don't apply it yet
                if (preset.renameSelectedCustomPatternIndex >= 1)
                {
                    preset.renameCustomPattern = customPatternPresets[preset.renameSelectedCustomPatternIndex];
                }
            }
            preset.renameCustomPattern = EditorGUILayout.TextField("Custom Pattern", preset.renameCustomPattern);
        }

        public virtual void DrawRegexFields(RenameToolPresetObject preset)
        {
            DrawRegexHelperUI("Naming Regex Template", ref preset.renameRegexPattern);
            preset.regexPrefix = EditorGUILayout.TextField("Prefix (Regex)", preset.regexPrefix);
            preset.regexSuffix = EditorGUILayout.TextField("Suffix (Regex)", preset.regexSuffix);
            preset.renameRegexPattern = EditorGUILayout.TextField("Naming Regex Pattern", preset.renameRegexPattern);
            EditorGUILayout.LabelField("Replacement Value");
            preset.newName = EditorGUILayout.TextField(preset.newName);
        }

        public virtual void DrawRegexHelperUI(string label, ref string regexPattern)
        {
            var reg = regexPattern;
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            int selectedRegexIndex = System.Array.FindIndex(regexTemplates, x => x.Value == reg);
            selectedRegexIndex = EditorGUILayout.Popup(selectedRegexIndex, regexTemplates.Select(r => r.Key).ToArray());
            if (selectedRegexIndex >= 0 && selectedRegexIndex < regexTemplates.Length)
            {
                regexPattern = regexTemplates[selectedRegexIndex].Value;
            }
        }
    
        public virtual void DrawDateTimeFormatSelection(RenameToolPresetObject preset)
        {
            EditorGUILayout.LabelField("Date and Time Formats", EditorStyles.boldLabel);
            preset.selectedDateFormatIndex = EditorGUILayout.Popup("Date Format", preset.selectedDateFormatIndex, dateFormatOptions);
            preset.selectedTimeFormatIndex = EditorGUILayout.Popup("Time Format", preset.selectedTimeFormatIndex, timeFormatOptions);
            
            preset.selectedDateFormat = dateFormatOptions[preset.selectedDateFormatIndex];
            preset.selectedTimeFormat = timeFormatOptions[preset.selectedTimeFormatIndex];
        }
    }
}
