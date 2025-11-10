using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Editor.Extensions.SerializedPropertyExtensions;
using PXE.Core.Editor.Objects;
using PXE.Core.Game.Managers;
using PXE.Core.State_System;
using PXE.Core.Utilities.Reflection;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Managers
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : ObjectControllerEditor
    {
        protected GameManager manager;
        protected SerializedProperty currentStateProperty;
        protected SerializedProperty initialStateProperty;
        protected SerializedProperty allStatesProperty;

        protected bool isEnteringState;
        protected GameState previousState;
        protected string[] cachedStates = null;
        protected bool isActive;
        protected Dictionary<string, Type> derivedStateLookup;

        public virtual void OnEnable()
        {
            currentStateProperty = serializedObject.FindBackingProperty(nameof(GameManager.CurrentState));
            initialStateProperty = serializedObject.FindBackingProperty(nameof(GameManager.InitialState));
            allStatesProperty = serializedObject.FindBackingProperty(nameof(GameManager.AllStates));

            // Populate the dictionary:
            derivedStateLookup = new Dictionary<string, Type>();
            var derivedTypes = ReflectionUtility.GetDerivedTypes(typeof(GameState));
            foreach (var type in derivedTypes)
            {
                derivedStateLookup[type.Name] = type;
            }

            // Refresh cached states
            cachedStates = GetAvailableStates();

            if (manager == null)
            {
                manager = (GameManager)target;
            }
        }

        public override void OnInspectorGUI()
        {
            // serializedObject.Update();
            
            DrawObjectControllerInspector();

            DrawDefaultInspector();

            cachedStates = GetAvailableStates(); // Refresh the cached states every time the inspector is drawn

            // If AllStates contains elements, process the logic. Otherwise, show warnings.
            if (manager.AllStates != null && manager.AllStates.Count > 0)
            {
                // Set default CurrentState and InitialState if they're null
                if (manager.CurrentState == null)
                {
                    manager.CurrentState = manager.AllStates[0];
                }

                if (manager.InitialState == null)
                {
                    manager.InitialState = manager.AllStates[0];
                }

                // Handle CurrentState drawing
                if (manager.AllStates is { Count: > 0 })
                {
                    var currentStateIndex = GetCurrentStateIndex(manager.CurrentState);
                    var newStateIndex = EditorGUILayout.Popup("Current State", currentStateIndex, cachedStates);
                    var newStateType = GetDerivedType(cachedStates[newStateIndex]);

                    if (newStateType != manager.CurrentState.GetType())
                    {
                        if (!isEnteringState)
                        {
                            isEnteringState = true;
                            manager.CurrentState = manager.AllStates.First(s => s.GetType() == newStateType);
                            isEnteringState = false;
                        }
                    }

                    EditorGUILayout.PropertyField(currentStateProperty);
                }
                else
                {
                    EditorGUILayout.HelpBox("CurrentState cannot be set as there are no available states in AllStates.", MessageType.Warning);
                }

                EditorGUILayout.Space();

                // Handle InitialState drawing
                if (manager.AllStates != null && manager.AllStates.Count > 0)
                {
                    var initialStateIndex = GetCurrentStateIndex(manager.InitialState);
                    var newInitialStateIndex = EditorGUILayout.Popup("Initial State", initialStateIndex, cachedStates);
                    if (initialStateIndex != newInitialStateIndex)
                    {
                        var newInitialStateType = GetDerivedType(cachedStates[newInitialStateIndex]);
                        var newStateInstance = manager.AllStates.First(s => s.GetType() == newInitialStateType);
                        initialStateProperty.objectReferenceValue = newStateInstance;
                    }

                    EditorGUILayout.PropertyField(initialStateProperty);
                }
                else
                {
                    EditorGUILayout.HelpBox("InitialState cannot be set as there are no available states in AllStates.", MessageType.Warning);
                }
                
                // Handle AllStates drawing
                EditorGUILayout.PropertyField(allStatesProperty, true); // The second argument 'true' ensures that children elements of the list are drawn
                if (manager.AllStates == null || manager.AllStates.Count == 0)
                {
                    EditorGUILayout.HelpBox("AllStates is not initialized or empty.", MessageType.Error);
                }
            }
            
            if (GUILayout.Button("Load States from Resources"))
            {
                manager.AllStates = Resources.LoadAll<GameState>("States").ToList();
                serializedObject.Update();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual string[] GetAvailableStates()
        {
            var manager = (GameManager)target;
            return manager.AllStates?.Select(s => s?.GetType().Name).ToArray() ?? Array.Empty<string>();
        }

        public virtual int GetCurrentStateIndex(GameState state)
        {
            var manager = (GameManager)target;
            for (var i = 0; i < manager.AllStates.Count; i++)
            {
                if (state.GetType() == manager.AllStates[i]?.GetType())
                {
                    return i;
                }
            }

            return 0;
        }

        public virtual Type GetDerivedType(string stateName)
        {
            var manager = (GameManager)target;
            var state = manager.AllStates?.FirstOrDefault(s => s.GetType().Name == stateName);
            return state?.GetType() ?? typeof(GameState);
        }
    }
}
