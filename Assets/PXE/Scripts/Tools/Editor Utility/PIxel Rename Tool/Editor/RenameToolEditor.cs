using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PXE.Core.Enums;
using PXE.Scripts.Tools.Editor_Utility.PIxel_Rename_Tool.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Tools.Editor_Utility.PIxel_Rename_Tool.Editor
{
    public class RenameToolEditor : EditorWindow
    {
        // Additional class properties for new placeholders
        public string projectId = string.Empty;
        public string version = string.Empty;
        public string platform = string.Empty;
        public string companyName = string.Empty;
        public string productName = string.Empty;
        public string resolution = string.Empty;
        
        public RenamingMode renamingMode = RenamingMode.Standard;
        public string prefix = string.Empty;
        public string suffix = string.Empty;
        public string regexPrefix = string.Empty;
        public string regexSuffix = string.Empty;
        public string newName = string.Empty;
        public IncrementalValueType incrementalType = IncrementalValueType.None;
        public string replaceNameRegexPattern = string.Empty;
        public string placementRegexPattern = string.Empty;
        public int startNumber = 1; // Starting number for numbered incrementation
        public string incrementalCustomPattern = "_{count}";
        public string renameCustomPattern = "{baseName}";
        public RenameToolPresetObject currentPreset;
        public bool useRegex = false; // For toggling the use of regex
        public bool showPreview = true; // For toggling the display of rename previews
        public PlacementStrategy placementStrategy = PlacementStrategy.AfterSuffix;
        public int specificIndex = 0;
        public string afterCharacter = string.Empty;
        public string beforeCharacter = string.Empty;
        public string afterWord = string.Empty;
        public string beforeWord = string.Empty;
        public string replaceCharacter = string.Empty;
        public string replaceWord = string.Empty;
        public int selectedRenameRegexPatternIndex = 0;
        public int selectedPlacementRegexPatternIndex = 0;
        
        public bool showBasicSettings = true;
        public bool showIncrementalNaming = true;
        public bool showPlacementStrategy = true;
        public bool showPresetManagement = true;
        public bool showPreviewAndRename = true;
        public Vector2 scrollPosition;
        
        public int incrementalSelectedCustomPatternIndex = 0; // Class-level field to store the selection
        public int renameSelectedCustomPatternIndex = 0; // Class-level field to store the selection
        public int selectedRenameRegexPresetIndex = 0; // Remember the selection
        public CasingType selectedCasingType = CasingType.Original;
        public RenamingMode previousRenamingMode = RenamingMode.Standard; // Default to match initial mode
        public bool isLoadingPreset = false; // Flag to track when loading a preset
        public int selectedDateFormatIndex = 0;
        public int selectedTimeFormatIndex = 0;
        public string selectedDateFormat = "yyyy-MM-dd"; // ISO 8601
        public string selectedTimeFormat = "HH:mm"; // 24-hour format

        
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
            // Date formats
            new KeyValuePair<string, string>("Date (dd-MM-yyyy)", @"\d{2}-\d{2}-\d{4}"),
            new KeyValuePair<string, string>("Date (MM-dd-yyyy)", @"\d{2}-\d{2}-\d{4}"),
            new KeyValuePair<string, string>("Date (yyyyMMdd)", @"\d{8}"),
            new KeyValuePair<string, string>("Date (dd MMM yyyy)", @"\d{2} [A-Za-z]{3} \d{4}"),
            new KeyValuePair<string, string>("Date (MMMM dd, yyyy)", @"[A-Za-z]+ \d{2}, \d{4}"),
            new KeyValuePair<string, string>("Date (dd MMMM yyyy)", @"\d{2} [A-Za-z]+ \d{4}"),
            new KeyValuePair<string, string>("Date (yyyy MMM dd)", @"\d{4} [A-Za-z]{3} \d{2}"),
            new KeyValuePair<string, string>("Date (yyyy MMMM dd)", @"\d{4} [A-Za-z]+ \d{2}"),
            new KeyValuePair<string, string>("Date (dd-MM-yy)", @"\d{2}-\d{2}-\d{2}"),
            new KeyValuePair<string, string>("Date (MM-dd-yy)", @"\d{2}-\d{2}-\d{2}"),
            new KeyValuePair<string, string>("Date (yy/MM/dd)", @"\d{2}/\d{2}/\d{2}"),
            new KeyValuePair<string, string>("Date (dd/MM/yy)", @"\d{2}/\d{2}/\d{2}"),
            // Time formats
            new KeyValuePair<string, string>("Time (HHmm)", @"\d{4}"),
            new KeyValuePair<string, string>("Time (hhmm tt)", @"\d{4} (AM|PM)"),
            new KeyValuePair<string, string>("Time (HH:mm:ss.fff)", @"\d{2}:\d{2}:\d{2}\.\d{3}"),
            new KeyValuePair<string, string>("Time (hh:mm:ss.fff tt)", @"\d{2}:\d{2}:\d{2}\.\d{3} (AM|PM)"),
            new KeyValuePair<string, string>("Time (HHmmss)", @"\d{6}"),
            new KeyValuePair<string, string>("Time (hhmmsstt)", @"\d{6}(AM|PM)"),
            new KeyValuePair<string, string>("Time (HH:mm:ss:fff)", @"\d{2}:\d{2}:\d{2}:\d{3}"),
            new KeyValuePair<string, string>("Time (hh:mm:ss:fff tt)", @"\d{2}:\d{2}:\d{2}:\d{3} (AM|PM)"),
            // IP Address
            new KeyValuePair<string, string>("IP Address", @"\b(?:\d{1,3}\.){3}\d{1,3}\b"),
            // Additional patterns as needed
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
        
        [MenuItem("PXE/Tools/Pixel Wizard Rename Tool")]
        public static void ShowWindow()
        {
            GetWindow<RenameToolEditor>(false, "Pixel Wizard Rename Tool", true);
        }

        public virtual void OnEnable()
        {
            projectId = Application.identifier;
            version = Application.version;
            platform = Application.platform.ToString();
            companyName = Application.companyName;
            productName = Application.productName;
            resolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        }

        public virtual void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Pixel Wizard Rename Tool", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configure the settings below to rename selected objects in the Unity Editor.", MessageType.Info);

            // Start of scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Preset Management Section
            showPresetManagement = EditorGUILayout.Foldout(showPresetManagement, "Preset Management", true);
            if (showPresetManagement)
            {
                DrawPresetManagement();
            }
            

            // Basic Settings Section
            showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "Basic Settings", true);
            
            if (showBasicSettings)
            {
                RenamingMode newRenamingMode = (RenamingMode)EditorGUILayout.EnumPopup("Renaming Mode", renamingMode);
                if (newRenamingMode != previousRenamingMode && !isLoadingPreset)
                {
                    //TODO: This needs fixing. currently changing the renaming mode when loading a preset clears the fields when it should not
                    ClearFields();
                    previousRenamingMode = newRenamingMode; // Update the previous mode to the current mode after clearing fields
                }
                renamingMode = newRenamingMode; // Update the current renaming mode based on the selection

                switch (renamingMode)
                {
                    case RenamingMode.Standard:
                        DrawStandardRenaming();
                        break;
                    case RenamingMode.Pattern:
                        DrawPatternRenaming();
                        break;
                    case RenamingMode.Regex:
                        DrawRegexRenaming();
                        break;
                }
                
                DrawDateTimeFormatSelection();
            }

            // Incremental Naming Section
            showIncrementalNaming = EditorGUILayout.Foldout(showIncrementalNaming, "Incremental Naming", true);
            if (showIncrementalNaming)
            {
                DrawIncrementalNaming();
            }
            
            if(incrementalType != IncrementalValueType.None)
            {
                // Placement Strategy Section
                showPlacementStrategy = EditorGUILayout.Foldout(showPlacementStrategy, "Placement Strategy", true);
                if (showPlacementStrategy)
                {
                    DrawPlacementStrategy();
                }
            }

            // Preview and Rename Section
            showPreviewAndRename = EditorGUILayout.Foldout(showPreviewAndRename, "Preview", true);
            if (showPreviewAndRename)
            {
                DrawPreview();
            }
            
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Rename", "Apply the renaming operation to selected objects.")))
            {
                RenameSelectedObjects();
            }
            // End of scroll view

            EditorGUILayout.EndScrollView();

        }
        
        public virtual void DrawDateTimeFormatSelection()
        {
            EditorGUILayout.LabelField("Date and Time Formats", EditorStyles.boldLabel);
            selectedDateFormatIndex = EditorGUILayout.Popup("Date Format", selectedDateFormatIndex, dateFormatOptions);
            selectedTimeFormatIndex = EditorGUILayout.Popup("Time Format", selectedTimeFormatIndex, timeFormatOptions);
            
            selectedDateFormat = dateFormatOptions[selectedDateFormatIndex];
            selectedTimeFormat = timeFormatOptions[selectedTimeFormatIndex];
        }

        public virtual void ClearFields()
        {
            prefix = string.Empty;
            suffix = string.Empty;
            regexPrefix = string.Empty;
            regexSuffix = string.Empty;
            newName = string.Empty;
            incrementalType = IncrementalValueType.None;
            replaceNameRegexPattern = string.Empty;
            placementRegexPattern = string.Empty;
            placementStrategy = PlacementStrategy.AfterSuffix;
            specificIndex = 0;
            afterCharacter = string.Empty;
            beforeCharacter = string.Empty;
            afterWord = string.Empty;
            beforeWord = string.Empty;
            replaceCharacter = string.Empty;
            replaceWord = string.Empty;
            incrementalCustomPattern = "_{count}";
            renameCustomPattern = "{baseName}";
            selectedCasingType = CasingType.Original;
            renameSelectedCustomPatternIndex = 0; 
            incrementalSelectedCustomPatternIndex = 0; 
            selectedRenameRegexPresetIndex = 0; 
            selectedPlacementRegexPatternIndex = 0;
        }

        public virtual void DrawPatternRenaming()
        {
            ShowRenameCustomPatternPresets(); // Existing method for pattern presets UI
            
            EditorGUILayout.LabelField("Naming Convention", EditorStyles.boldLabel);
            selectedCasingType = (CasingType)EditorGUILayout.EnumPopup("Casing Type", selectedCasingType);
        }

        public virtual void DrawRegexRenaming()
        {
            // Use the class variable for the selected index and update it based on user selection
            int newIndex = EditorGUILayout.Popup("Select Regex Template", selectedRenameRegexPresetIndex, regexTemplates.Select(r => r.Key).ToArray());
    
            // Only update the regex pattern if the selection has changed
            if (newIndex != selectedRenameRegexPresetIndex)
            {
                selectedRenameRegexPresetIndex = newIndex;
                if (selectedRenameRegexPresetIndex > 0)
                {
                    replaceNameRegexPattern = regexTemplates[selectedRenameRegexPresetIndex].Value;
                }
            }

            EditorGUILayout.LabelField("Regex Pattern");
            // Always allow the user to edit replaceNameRegexPattern
            replaceNameRegexPattern = EditorGUILayout.TextField(replaceNameRegexPattern);

            EditorGUILayout.LabelField(new GUIContent("Prefix", "Add this prefix to the beginning of each selected object's name."));
            regexPrefix = EditorGUILayout.TextField(regexPrefix);
            EditorGUILayout.LabelField(new GUIContent("Suffix", "Add this suffix to the end of each selected object's name."));
            regexSuffix = EditorGUILayout.TextField(regexSuffix);

            EditorGUILayout.LabelField("Replacement Value");
            newName = EditorGUILayout.TextField(newName);
            
            EditorGUILayout.LabelField("Naming Convention", EditorStyles.boldLabel);
            selectedCasingType = (CasingType)EditorGUILayout.EnumPopup("Casing Type", selectedCasingType);
        }



        public virtual void DrawStandardRenaming()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Basic Settings", EditorStyles.boldLabel);
    
            EditorGUILayout.LabelField(new GUIContent("Prefix", "Add this prefix to the beginning of each selected object's name."));
            prefix = EditorGUILayout.TextField(prefix);
    
            EditorGUILayout.LabelField(new GUIContent("Suffix", "Add this suffix to the end of each selected object's name."));
            suffix = EditorGUILayout.TextField(suffix);
    
            EditorGUILayout.LabelField(new GUIContent("New Name", "The new base name for selected objects. Leave empty to retain original names."));
            newName = EditorGUILayout.TextField(newName);
            
            EditorGUILayout.LabelField("Naming Convention", EditorStyles.boldLabel);
            selectedCasingType = (CasingType)EditorGUILayout.EnumPopup("Casing Type", selectedCasingType);
        }
        
        public virtual void DrawPresetManagement()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Preset Management", EditorStyles.boldLabel);
            currentPreset = EditorGUILayout.ObjectField(new GUIContent("Current Preset", "Select a preset to load or save settings."), currentPreset, typeof(RenameToolPresetObject), false) as RenameToolPresetObject;

            if (currentPreset == null)
            {
                if (GUILayout.Button("Create New Preset"))
                {
                    CreateAndSaveNewPreset();
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Load Preset", "Load the selected preset settings.")))
                {
                    LoadPreset();
                }
                if (GUILayout.Button(new GUIContent("Save To Current Preset", "Save the current settings to the selected preset.")))
                {
                    SaveCurrentSettingsToPreset();
                }
            }
        }
        
        public virtual void DrawPreview()
        {
            showPreview = EditorGUILayout.Toggle(new GUIContent("Show Rename Previews", "Toggle to show or hide rename previews."), showPreview);
            if (showPreview)
            {
                ShowSelectionInfo();
                ShowRenamePreview();
            }
        }

        public virtual void DrawIncrementalNaming()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Incremental Naming", EditorStyles.boldLabel);
            incrementalType = (IncrementalValueType)EditorGUILayout.EnumPopup(new GUIContent("Incremental Value Type", "Select how to incrementally label selected objects."), incrementalType);
            if (incrementalType == IncrementalValueType.Numbered || incrementalType == IncrementalValueType.RomanNumerals || incrementalType == IncrementalValueType.Hexadecimal)
            {
                startNumber = EditorGUILayout.IntField(new GUIContent("Start Number", "The starting number for the incremental count."), startNumber);
            }
            if (incrementalType == IncrementalValueType.CustomPattern)
            {
                ShowCustomIncrementTypeOptions();
            }
        }

        public virtual void DrawPlacementStrategy()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Placement Strategy", EditorStyles.boldLabel);
            ShowCustomPlacementOptions(); // Adjusted to potentially include a regex field for placement when "Regex Pattern" is selected
            if (placementStrategy == PlacementStrategy.RegexPattern)
            {
                ShowRegexPatternHelper(); // For placement strategy-specific regex
            }
        }

        public virtual void ShowCustomIncrementTypeOptions()
        {
            EditorGUILayout.HelpBox("Use placeholders in your pattern: {count}, {name}, {date}, etc. For patterns, use the Pattern below.", MessageType.Info);
            ShowIncrementalCustomPatternPresets();
        }

        public virtual void ShowCustomPlacementOptions()
        {
            EditorGUILayout.LabelField("Custom Placement Configuration", EditorStyles.boldLabel);
            placementStrategy = (PlacementStrategy)EditorGUILayout.EnumPopup("Placement Strategy", placementStrategy);

            // Show additional fields based on the selected placement strategy
            switch (placementStrategy)
            {
                case PlacementStrategy.SpecificIndex:
                    specificIndex = EditorGUILayout.IntField("Specific Index", specificIndex);
                    break;
                case PlacementStrategy.AfterCharacter:
                    afterCharacter = EditorGUILayout.TextField("After Character", afterCharacter);
                    break;
                case PlacementStrategy.BeforeCharacter:
                    beforeCharacter = EditorGUILayout.TextField("Before Character", beforeCharacter);
                    break;
                case PlacementStrategy.AfterWord:
                    afterWord = EditorGUILayout.TextField("After Word", afterWord);
                    break;
                case PlacementStrategy.BeforeWord:
                    beforeWord = EditorGUILayout.TextField("Before Word", beforeWord);
                    break;
                case PlacementStrategy.ReplaceCharacter:
                    replaceCharacter = EditorGUILayout.TextField("Replace Character", replaceCharacter);
                    break;
                case PlacementStrategy.ReplaceWord:
                    replaceWord = EditorGUILayout.TextField("Replace Word", replaceWord);
                    break;
                case PlacementStrategy.RegexPattern:
                    break;
            }
        }

        public void LoadPreset()
        {
            if (currentPreset != null)
            {
                Undo.RecordObject(this, "Load Rename Tool Preset");

                isLoadingPreset = true; // Set the flag to prevent clearing fields when changing the renaming mode
                renamingMode = currentPreset.renamingMode;
                prefix = currentPreset.prefix;
                suffix = currentPreset.suffix;
                newName = currentPreset.newName;
                replaceNameRegexPattern = currentPreset.renameRegexPattern;
                placementRegexPattern = currentPreset.placementRegexPattern;
                startNumber = currentPreset.startNumber;
                incrementalCustomPattern = currentPreset.incrementalCustomPattern;
                renameCustomPattern = currentPreset.renameCustomPattern;
                incrementalType = currentPreset.incrementalType;
                placementStrategy = currentPreset.placementStrategy;
                regexPrefix = currentPreset.regexPrefix;
                regexSuffix = currentPreset.regexSuffix;
                selectedRenameRegexPresetIndex = Array.FindIndex(regexTemplates, r => r.Value == replaceNameRegexPattern);
                selectedRenameRegexPatternIndex = Array.FindIndex(regexTemplates, r => r.Value == replaceNameRegexPattern);
                selectedCasingType = currentPreset.casingType;
                previousRenamingMode = currentPreset.renamingMode;
                selectedTimeFormat = currentPreset.selectedTimeFormat;
                selectedDateFormat = currentPreset.selectedDateFormat;
                selectedDateFormatIndex = Array.IndexOf(dateFormatOptions, selectedDateFormat);
                selectedTimeFormatIndex = Array.IndexOf(timeFormatOptions, selectedTimeFormat);
                renameSelectedCustomPatternIndex = currentPreset.renameSelectedCustomPatternIndex;
                incrementalSelectedCustomPatternIndex = currentPreset.incrementalSelectedCustomPatternIndex;

                // Mark the object as needing a repaint to ensure the Inspector updates
                EditorUtility.SetDirty(this);

                // If you're within an EditorWindow, calling Repaint should force the window to redraw
                Repaint();

                // For custom Editor scripts, you may need to signal the Inspector to redraw
                if (!EditorApplication.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                }
                isLoadingPreset = false; // Reset the flag after loading the preset
            }
            else
            {
                Debug.LogWarning("No preset selected.");
            }
        }
        
        public virtual void SaveCurrentSettingsToPreset()
        {
            if (currentPreset != null)
            {
                Undo.RecordObject(currentPreset, "Save Rename Tool Preset"); // Enable undo for preset changes
        
                // Update the preset object with the current editor settings
                currentPreset.renamingMode = renamingMode;
                currentPreset.prefix = prefix;
                currentPreset.suffix = suffix;
                currentPreset.newName = newName;
                currentPreset.renameRegexPattern = replaceNameRegexPattern;
                currentPreset.placementRegexPattern = placementRegexPattern;
                currentPreset.renameCustomPattern = renameCustomPattern;
                currentPreset.incrementalCustomPattern = incrementalCustomPattern;
                currentPreset.incrementalType = incrementalType;
                currentPreset.startNumber = startNumber;
                currentPreset.placementStrategy = placementStrategy;
                currentPreset.regexPrefix = regexPrefix;
                currentPreset.regexSuffix = regexSuffix;
                currentPreset.casingType = selectedCasingType;
                currentPreset.selectedDateFormat = selectedDateFormat;
                currentPreset.selectedTimeFormat = selectedTimeFormat;
                currentPreset.selectedDateFormatIndex = selectedDateFormatIndex;
                currentPreset.selectedTimeFormatIndex = selectedTimeFormatIndex;
                currentPreset.incrementalSelectedCustomPatternIndex = incrementalSelectedCustomPatternIndex;
                currentPreset.renameSelectedCustomPatternIndex = renameSelectedCustomPatternIndex;

                EditorUtility.SetDirty(currentPreset); // Mark the preset as dirty to ensure it's saved
            }
            else
            {
                Debug.LogWarning("No preset selected for saving.");
            }
        }

        
        public virtual void RenameSelectedObjects()
        {
            var selectedObjects = Selection.objects;
            bool encounteredError = false;
            List<string> errorMessages = new List<string>();
            int count = startNumber;
            DateTime now = DateTime.Now;

            foreach (var obj in selectedObjects)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string baseName = AssetDatabase.IsValidFolder(assetPath) ? new DirectoryInfo(assetPath).Name : obj.name;
                string finalName = CalculateFinalName(obj, baseName, ref count, now); // Assume this method exists and implements your renaming logic
                
                (bool isValid, string message) validationResult = (true, string.Empty);
                
                if (obj is MonoScript monoScript)
                {
                    validationResult = ValidateScriptName(finalName);
                    if (validationResult.isValid)
                    {
                        try
                        {
                            UpdateScriptClassNameAndAsset(monoScript, finalName);
                            continue; // Skip further renaming logic since UpdateScriptClassNameAndAsset handles it
                        }
                        catch (Exception ex)
                        {
                            validationResult = (false, ex.Message);
                        }
                    }
                }
                else if (AssetDatabase.Contains(obj) || AssetDatabase.IsValidFolder(assetPath))
                {
                    validationResult = ValidateAssetName(finalName);
                }
                else if (obj is GameObject)
                {
                    validationResult = ValidateGameObjectName(finalName);
                }
                
                // Extend with other type checks as necessary

                if (!validationResult.isValid)
                {
                    encounteredError = true;
                    errorMessages.Add($"Error renaming '{obj.name}' to '{finalName}': {validationResult.message}");
                    continue;
                }

                // If validation passed, apply renaming logic
                ApplyRenameLogic(assetPath, finalName, obj);
            }

            if (encounteredError)
            {
                string errorMessage = string.Join("\n", errorMessages);
                EditorUtility.DisplayDialog("Renaming Errors", $"Some items were not renamed:\n{errorMessage}", "OK");
            }
            else
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        public virtual string CalculateFinalName(UnityEngine.Object obj, string baseName, ref int count, DateTime now)
        {
            string finalName = baseName; // Default to base name

            // Handle different renaming modes
            switch (renamingMode)
            {
                case RenamingMode.Standard:
                    // Standard renaming logic
                    finalName = (!string.IsNullOrWhiteSpace(newName)) ? newName : baseName;
                    finalName = prefix + finalName + suffix; // Apply optional prefix and suffix
                    break;

                case RenamingMode.Pattern:
                    // Apply custom pattern renaming
                    finalName = ApplyCustomPattern(renameCustomPattern, baseName, count, now, obj.name);
                    break;

                case RenamingMode.Regex:
                    // Regex mode renaming, apply only if pattern matches
                    if (Regex.IsMatch(baseName, replaceNameRegexPattern))
                    {
                        finalName = Regex.Replace(baseName, replaceNameRegexPattern, newName);
                        finalName = regexPrefix + finalName + regexSuffix; // Optionally add prefix and suffix
                    }
                    else
                    {
                        // Skip renaming if pattern doesn't match (handled outside this method)
                        return null;
                    }
                    break;
            }

            // Apply incremental naming if applicable
            if (incrementalType != IncrementalValueType.None)
            {
                var (incrementalBaseName, incrementalValue) = ApplyIncrementalName(finalName, ref count, now, incrementalCustomPattern, finalName);
                finalName = ApplyPlacementStrategy(incrementalBaseName, incrementalValue);
            }

            // Apply selected casing
            finalName = ApplyCasing(finalName, selectedCasingType);

            return finalName;
        }
        
        public virtual void UpdateScriptClassNameAndAsset(MonoScript monoScript, string newFileName)
        {
            string assetPath = AssetDatabase.GetAssetPath(monoScript);
            // Ensure the path is correct for all operating systems
            string fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), assetPath);
            string scriptText = File.ReadAllText(fullPath);

            string originalClassName = monoScript.name;
            // Generate the new class name, ensuring it follows the PascalCase convention or includes underscores only if explicitly added
            string newClassName = GetFormattedClassName(Path.GetFileNameWithoutExtension(newFileName));

            string pattern = $@"\bclass\s+{originalClassName}\s*\b";
            string replacement = $"class {newClassName}";

            // Perform the replacement
            string updatedScriptText = Regex.Replace(scriptText, pattern, replacement, RegexOptions.Multiline);

            // Check if the replacement was successful by comparing the updated script text with the original
            if (!updatedScriptText.Equals(scriptText))
            {
                // Write the updated script text back to the file
                File.WriteAllText(fullPath, updatedScriptText);

                // Update the asset name to reflect the new class name. Unity uses the file name as the class name in the editor.
                AssetDatabase.RenameAsset(assetPath, newClassName.Replace(" ", "")); // Ensure no spaces are included in the file name
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                EditorUtility.DisplayDialog("Script Update Failed", $"Failed to update the class name in the script from '{originalClassName}' to '{newClassName}'.", "OK");
            }
        }

        public virtual string GetFormattedClassName(string className)
        {
            // If the class name explicitly includes underscores, return it directly without changes
            if (className.Contains("_"))
            {
                return className;
            }

            // Otherwise, format the class name to PascalCase
            return ConvertToPascalCase(className);
        }

        public virtual string ConvertToPascalCase(string input)
        {
            // Split the string into words, remove invalid characters, and capitalize the first letter of each word
            var words = Regex.Split(input, @"[^a-zA-Z0-9_]+").Where(word => word.Length > 0);
            return string.Concat(words.Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }

        public virtual (bool isValid, string message) ValidateScriptName(string name)
        {
            // Add validation logic specific to script names, if any
            return (true, string.Empty);
        }

        private void ApplyRenameLogic(string assetPath, string finalName, UnityEngine.Object obj)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                // Folder renaming logic
                string parentFolderPath = Path.GetDirectoryName(assetPath);
                string newFolderPath = Path.Combine(parentFolderPath, finalName);
                AssetDatabase.MoveAsset(assetPath, newFolderPath);
            }
            else
            {
                // Non-folder object renaming logic
                if (AssetDatabase.Contains(obj))
                {
                    AssetDatabase.RenameAsset(assetPath, finalName);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
                else
                {
                    GameObject go = obj as GameObject;
                    if (go != null)
                    {
                        Undo.RecordObject(go, "Rename Objects");
                        go.name = finalName;
                    }
                }
            }
        }

        
        public virtual string NextAlphabetic(int index)
        {
            string result = "";
            while (index > 0)
            {
                index--; // Adjust for 0-indexing
                int remainder = index % 26;
                char letter = (char)('A' + remainder);
                result = letter + result;
                index = (index - remainder) / 26;
            }

            return result;
        }

        
        public virtual string NumberToRoman(int number)
        {
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + NumberToRoman(number - 1000);
            if (number >= 900) return "CM" + NumberToRoman(number - 900);
            if (number >= 500) return "D" + NumberToRoman(number - 500);
            if (number >= 400) return "CD" + NumberToRoman(number - 400);
            if (number >= 100) return "C" + NumberToRoman(number - 100);
            if (number >= 90) return "XC" + NumberToRoman(number - 90);
            if (number >= 50) return "L" + NumberToRoman(number - 50);
            if (number >= 40) return "XL" + NumberToRoman(number - 40);
            if (number >= 10) return "X" + NumberToRoman(number - 10);
            if (number >= 9) return "IX" + NumberToRoman(number - 9);
            if (number >= 5) return "V" + NumberToRoman(number - 5);
            if (number >= 4) return "IV" + NumberToRoman(number - 4);
            if (number >= 1) return "I" + NumberToRoman(number - 1);
            return string.Empty;
        }


        public virtual (string baseName, string incrementalValue) ApplyIncrementalName(string baseName, ref int count, DateTime now, string pattern, string objectName)
        {
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = objectName; // Use the existing object name if new name is blank
            }
            
            string incrementalValue = "";
            string finalBaseName = baseName; // Default to the base name

            switch (incrementalType)
            {
                case IncrementalValueType.Numbered:
                    incrementalValue = count.ToString();
                    count++;
                    break;
                case IncrementalValueType.Alphabetically:
                    // Directly use 'count' to generate the alphabetic sequence
                    incrementalValue = NextAlphabetic(count);
                    count++; // Increment count for the next usage
                    break;
                case IncrementalValueType.DateTimeStamp:
                    switch (selectedDateFormat)
                    {
                        case "None" when selectedTimeFormat == "None":
                            incrementalValue = ""; // No date or time to be included
                            break;
                        case "None":
                            incrementalValue = now.ToString(selectedTimeFormat); // Include only time
                            break;
                        default:
                        {
                            incrementalValue = now.ToString(selectedTimeFormat == "None" ? selectedDateFormat : // Include only date
                                $"{selectedDateFormat} {selectedTimeFormat}"); // Include both date and time
                            break;
                        }
                    }                    
                    break;
                case IncrementalValueType.RomanNumerals:
                    incrementalValue = NumberToRoman(count);
                    count++; // Increment count for the next usage
                    break;
                case IncrementalValueType.Hexadecimal:
                    incrementalValue = count.ToString("X");
                    count++; // Increment count for the next usage
                    break;
                case IncrementalValueType.UniqueID:
                    incrementalValue = Guid.NewGuid().ToString("N").Substring(0, 8);
                    break;
                case IncrementalValueType.CustomPattern:
                    // Replace placeholders in the custom pattern with actual values
                    incrementalValue = ApplyCustomPattern(pattern, baseName, count, now, objectName);
                    count++; // Increment count for the next usage
                    break;
            }

            if (renamingMode == RenamingMode.Regex)
            {
                finalBaseName = !string.IsNullOrWhiteSpace(newName) ? regexPrefix + newName + regexSuffix : regexPrefix + baseName + regexSuffix;
            }
            else
            {
                // If newName is provided, it overrides the baseName for the renaming process
                finalBaseName = !string.IsNullOrWhiteSpace(newName) ? newName : baseName;
            }

            return (finalBaseName, incrementalValue);
        }

        public virtual string ApplyCustomPattern(string pattern, string baseName, int count, DateTime now, string objectName) 
        {

            string result = pattern
                .Replace("{baseName}", baseName)
                .Replace("{count}", count.ToString("D3"))
                .Replace("{year}", now.Year.ToString())
                .Replace("{month}", now.Month.ToString("D2"))
                .Replace("{day}", now.Day.ToString("D2"))
                .Replace("{hour}", now.Hour.ToString("D2"))
                .Replace("{minute}", now.Minute.ToString("D2"))
                .Replace("{second}", now.Second.ToString("D2"))
                .Replace("{newName}", newName)
                .Replace("{objectName}", objectName)
                .Replace("{guid}", Guid.NewGuid().ToString())
                .Replace("{projectId}", projectId)
                .Replace("{version}", version)
                .Replace("{platform}", platform)
                .Replace("{resolution}", resolution)
                .Replace("{companyName}", companyName)
                .Replace("{productName}", productName);
            
            // Apply date format if not 'None'
            if (selectedDateFormat != "None") 
            {
                string formattedDate = now.ToString(selectedDateFormat);
                result = result.Replace("{date}", formattedDate);
            } 
            else 
            {
                result = result.Replace("{date}", ""); // Remove placeholder if 'None'
            }

            // Apply time format if not 'None'
            if (selectedTimeFormat != "None") {
                string formattedTime = now.ToString(selectedTimeFormat);
                result = result.Replace("{time}", formattedTime);
            } 
            else 
            {
                result = result.Replace("{time}", ""); // Remove placeholder if 'None'
            }

            return result;
        }

        
        public virtual string ApplyPlacementStrategy(string baseName, string incrementalValue)
        {
            switch (placementStrategy)
            {
                case PlacementStrategy.BeforePrefix:
                    return incrementalValue + prefix + baseName + suffix;
                case PlacementStrategy.AfterPrefix:
                    return prefix + incrementalValue + baseName + suffix;
                case PlacementStrategy.BeforeSuffix:
                    return prefix + baseName + incrementalValue + suffix;
                case PlacementStrategy.AfterSuffix:
                    return prefix + baseName + suffix + incrementalValue;
                case PlacementStrategy.SpecificIndex:
                    if (specificIndex >= 0 && specificIndex <= baseName.Length)
                    {
                        return baseName.Insert(specificIndex, incrementalValue);
                    }
                    break;
                case PlacementStrategy.AfterCharacter:
                    int indexAfterChar = baseName.IndexOf(afterCharacter) + afterCharacter.Length;
                    if (indexAfterChar >= afterCharacter.Length)
                    {
                        return baseName.Insert(indexAfterChar, incrementalValue);
                    }
                    break;
                case PlacementStrategy.BeforeCharacter:
                    int indexBeforeChar = baseName.IndexOf(beforeCharacter);
                    if (indexBeforeChar != -1)
                    {
                        return baseName.Insert(indexBeforeChar, incrementalValue);
                    }
                    break;
                case PlacementStrategy.AfterWord:
                    int indexAfterWord = baseName.IndexOf(afterWord) + afterWord.Length;
                    if (indexAfterWord >= afterWord.Length)
                    {
                        return baseName.Insert(indexAfterWord + (afterWord.Length > 0 ? 1 : 0), incrementalValue);
                    }
                    break;
                case PlacementStrategy.BeforeWord:
                    int indexBeforeWord = baseName.IndexOf(beforeWord);
                    if (indexBeforeWord != -1)
                    {
                        return baseName.Insert(indexBeforeWord, incrementalValue);
                    }
                    break;
                case PlacementStrategy.ReplaceCharacter:
                    if (baseName.Contains(replaceCharacter) && replaceCharacter.Length > 0)
                    {
                        return baseName.Replace(replaceCharacter, incrementalValue);
                    }
                    break;
                case PlacementStrategy.ReplaceWord:
                    if (baseName.Contains(replaceWord) && replaceWord.Length > 0)
                    {
                        return baseName.Replace(replaceWord, incrementalValue);
                    }
                    break;
                case PlacementStrategy.RegexPattern:
                    if (!string.IsNullOrEmpty(placementRegexPattern))
                    {
                        if(Regex.IsMatch(baseName, placementRegexPattern))
                        {
                            return Regex.Replace(baseName, placementRegexPattern, incrementalValue);
                        }
                    }
                    break;
            }

            // Fallback if no valid placement strategy is found or if specific index is out of range
            return baseName + incrementalValue;
        }

        
        public virtual void CreateAndSaveNewPreset()
        {
            RenameToolPresetObject newPreset = ScriptableObject.CreateInstance<RenameToolPresetObject>();
            string path = EditorUtility.SaveFilePanelInProject("Save New Preset", "NewRenameToolPreset", "asset", "Please enter a file name to save the new preset.");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newPreset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                currentPreset = newPreset;
                SaveCurrentSettingsToPreset(); // Optionally, save current settings to the newly created preset.
            }
        }
        
        public virtual void ShowSelectionInfo()
        {
            var selectedObjects = Selection.objects;
            GUILayout.Label($"Selected Items: {selectedObjects.Length}", EditorStyles.boldLabel);

            if (selectedObjects.Length > 0)
            {
                var groupedObjects = selectedObjects.GroupBy(obj => obj.GetType().Name)
                                                    .Select(group => $"{group.Key}: {group.Count()}")
                                                    .ToArray();
                string selectionDetails = string.Join(", ", groupedObjects);
                GUILayout.Label($"Types: {selectionDetails}", EditorStyles.miniLabel);
            }
        }

        public virtual void ShowRenamePreview()
        {
            var selectedObjects = Selection.objects;
            if (selectedObjects.Length == 0)
            {
                GUILayout.Label("No objects selected for preview.", EditorStyles.miniLabel);
                return;
            }

            GUILayout.Label("Rename Preview:", EditorStyles.boldLabel);

            int previewCount = Mathf.Min(selectedObjects.Length, 10); // Limit preview count for performance/readability
            for (int i = 0; i < previewCount; i++)
            {
                var obj = selectedObjects[i];
                string originalName = obj.name;
                string previewName = GeneratePreviewName(obj.name, i + 1); // Assuming 'i + 1' as a simple increment for preview purposes

                GUILayout.Label($"{originalName} => {previewName}", EditorStyles.miniLabel);
            }

            if (selectedObjects.Length > 10)
            {
                GUILayout.Label("...and more", EditorStyles.miniLabel);
            }
        }
        
        public virtual string GeneratePreviewName(string originalName, int index)
        {
            DateTime now = DateTime.Now;
            int count = startNumber + index - 1; // Simulate count for preview
            string previewName = originalName;
            
            if (renamingMode == RenamingMode.Pattern)
            {
                previewName = ApplyCustomPattern(renameCustomPattern, previewName, count, now, previewName);
            }
            else if (renamingMode == RenamingMode.Regex)
            {
                // Regex mode renaming, check if baseName matches the pattern
                if (Regex.IsMatch(previewName, replaceNameRegexPattern))
                {
                    previewName = Regex.Replace(previewName, replaceNameRegexPattern, newName);
                    previewName = regexPrefix + previewName + regexSuffix; // Apply optional prefix and suffix
                }
                else
                {
                    previewName = "No match found for the regex pattern.";
                    return previewName;
                }
            }
                
            // Apply incremental naming if applicable
            var (incrementalBaseName, incrementalValue) = ApplyIncrementalName(previewName, ref count, now, incrementalCustomPattern, previewName);

                
            // Apply the placement strategy to get the final name
            previewName = ApplyPlacementStrategy(incrementalBaseName, incrementalValue);
                
            previewName = ApplyCasing(previewName, selectedCasingType);
            
            return previewName;
        }
        
        
        // Updated ShowCustomPatternPresets method
        public virtual void ShowIncrementalCustomPatternPresets()
        {
            EditorGUILayout.LabelField("Pattern Presets", EditorStyles.boldLabel);

            // Temporary index to hold the dropdown selection
            int newIndex = EditorGUILayout.Popup("Select Preset", incrementalSelectedCustomPatternIndex, customPatternPresets);
            if (newIndex != incrementalSelectedCustomPatternIndex)
            {
                incrementalSelectedCustomPatternIndex = newIndex;
                // Update the customPattern with the selected preset for preview, but don't apply it yet
                if (incrementalSelectedCustomPatternIndex >= 1)
                {
                    incrementalCustomPattern = customPatternPresets[incrementalSelectedCustomPatternIndex];
                }
            }
            
            EditorGUILayout.LabelField("Pattern");
            incrementalCustomPattern = EditorGUILayout.TextField(incrementalCustomPattern); // Directly edit the pattern here
        }
        
        public virtual void ShowRenameCustomPatternPresets()
        {
            EditorGUILayout.LabelField("Pattern Presets", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use placeholders in your pattern: {count}, {name}, {date}, etc. For patterns, use the Pattern below.", MessageType.Info);
            // Temporary index to hold the dropdown selection
            int newIndex = EditorGUILayout.Popup("Select Preset", renameSelectedCustomPatternIndex, customPatternPresets);
            if (newIndex != renameSelectedCustomPatternIndex)
            {
                renameSelectedCustomPatternIndex = newIndex;
                // Update the customPattern with the selected preset for preview, but don't apply it yet
                if (renameSelectedCustomPatternIndex >= 1)
                {
                    renameCustomPattern = customPatternPresets[renameSelectedCustomPatternIndex];
                }
            }
            
            EditorGUILayout.LabelField("Custom Pattern");
            renameCustomPattern = EditorGUILayout.TextField(renameCustomPattern); // Directly edit the custom pattern here
        }
        
        // Method to display regex helper for placement strategy
        public virtual void ShowRegexPatternHelper()
        {
            if (placementStrategy == PlacementStrategy.RegexPattern)
            {
                EditorGUILayout.LabelField("Placement Strategy Regex Helper", EditorStyles.boldLabel);
                
                // Use the class variable for the selected index and update it based on user selection
                int newIndex = EditorGUILayout.Popup("Select Regex Template", selectedPlacementRegexPatternIndex, regexTemplates.Select(r => r.Key).ToArray());
    
                // Only update the regex pattern if the selection has changed
                if (newIndex != selectedPlacementRegexPatternIndex)
                {
                    selectedPlacementRegexPatternIndex = newIndex;
                    if (selectedPlacementRegexPatternIndex > 0)
                    {
                        placementRegexPattern = regexTemplates[selectedPlacementRegexPatternIndex].Value;
                    }
                }

                // Displaying label for the selected regex pattern for placement
                EditorGUILayout.LabelField("Selected Regex for Placement");
                placementRegexPattern = EditorGUILayout.TextField(placementRegexPattern);
            }

            
        }
        
        // Method to validate and correct folder names
        public string ValidateFolderName(string folderName)
        {
            // List of characters that are not allowed in file/folder names in Windows and Unix/Linux
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
    
            // Replace invalid characters with an underscore or any character you see fit
            foreach (char c in invalidChars)
            {
                folderName = folderName.Replace(c, '_');
            }

            // Additional checks can be added here, e.g., for reserved names

            return folderName;
        }
        
        public string ApplyCasing(string input, CasingType casingType)
        {
            string normalizedInput = input.Trim();

            var words = Regex.Matches(input, @"[^\s]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            switch (casingType)
            {
                case CasingType.UpperCase:
                    return normalizedInput.ToUpper();
                case CasingType.LowerCase:
                    return normalizedInput.ToLower();
                case CasingType.CamelCase:
                    return words.Select((word, index) => index > 0 ? char.ToUpper(word[0]) + word.Substring(1).ToLower() : word.ToLower())
                        .Aggregate(string.Empty, (current, next) => current + next);
                case CasingType.PascalCase:
                    return words.Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                        .Aggregate(string.Empty, (current, next) => current + next);
                case CasingType.SnakeCase:
                    return string.Join("_", words).ToLower();
                case CasingType.KebabCase:
                    return string.Join("-", words).ToLower();
                case CasingType.MacroCase:
                    return string.Join("_", words).ToUpper();
                case CasingType.DotCase:
                    return string.Join(".", words).ToLower();
                case CasingType.SlashCase:
                    return string.Join("/", words).ToLower();
                case CasingType.BackslashCase:
                    return string.Join("\\", words).ToLower();
                case CasingType.TitleCase:
                    return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
                case CasingType.ColonCase:
                    return string.Join(":", words);
                case CasingType.LowerColonCase:
                    return string.Join(":", words).ToLower();
                case CasingType.UpperColonCase:
                    return string.Join(":", words).ToUpper();
                
                default:
                    return input; // Original casing
            }
        }
        
        public virtual (bool isValid, string message) ValidateAssetName(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (name.IndexOfAny(invalidChars) >= 0)
            {
                return (false, $"Name contains invalid characters: {name}\nInvalid characters: {string.Join(", ", invalidChars)}");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Name cannot be empty or whitespace.");
            }
            // Additional asset-specific checks here
            return (true, string.Empty);
        }

        public virtual (bool isValid, string message) ValidateGameObjectName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Name cannot be empty or whitespace.");
            }
            // Additional GameObject-specific checks here
            return (true, string.Empty);
        }

    }
}
