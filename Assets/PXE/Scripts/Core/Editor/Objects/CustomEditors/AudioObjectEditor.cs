using PXE.Core.Audio;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Editor.Objects
{
    [CustomEditor(typeof(AudioObject))]
    public class AudioObjectEditor : UnityEditor.Editor
    {
        private bool clipChanged = true;

        public override void OnInspectorGUI()
        {
            // Get the AudioObject scriptable object
            AudioObject audioObject = (AudioObject)target;

            // Cache the current clip before drawing the default inspector
            AudioClip previousClip = audioObject.Clip;

            // Draw the default inspector
            DrawDefaultInspector();

            // Check if the AudioClip reference has changed
            if (previousClip != audioObject.Clip)
            {
                clipChanged = true;
            }

            // If the AudioClip has changed and the Name property hasn't been set yet
            if (clipChanged && audioObject.Clip != null && audioObject.Name != audioObject.Clip.name)
            {
                // Update the Name property
                audioObject.Name = audioObject.Clip.name;

                // Rename the ScriptableObject file
                string assetPath = AssetDatabase.GetAssetPath(audioObject);
                AssetDatabase.RenameAsset(assetPath, audioObject.Name);
                
                // Reset the flag
                clipChanged = false;
            }

            // Add a button to rename the AudioClip
            if (GUILayout.Button("Rename AudioClip to Name"))
            {
                if (audioObject.Clip == null) return;
                //TODO: Extract this method to a utility class
                string clipPath = AssetDatabase.GetAssetPath(audioObject.Clip);
                AssetDatabase.RenameAsset(clipPath, audioObject.Name);
            }
        }
    }
}