using PXE.Core.Data_Persistence.Interfaces;
using UnityEditor;
using UnityEngine;

namespace PXE.Core.Data_Persistence.Editor
{
    [CustomEditor(typeof(BaseGameDataHandlerObject), true)]
    public class GameDataHandlerObjectEditor : UnityEditor.Editor
    {
        protected IGameDataHandlerObject GameDataHandler => (IGameDataHandlerObject)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if(GUILayout.Button("Set Data Handler"))
            {
                GameDataHandler.SetDataHandler(GameDataHandler.DataHandler.DataPath,
                    GameDataHandler.DataHandler.FileName,
                    GameDataHandler.DataHandler.Extension,
                    GameDataHandler.DataHandler.UseEncryption);
            }

            if (GUILayout.Button("Get Persistent Data Path"))
            {
                GameDataHandler.DataHandler.DataPath = Application.persistentDataPath;
                serializedObject.Update();
            }

            if (GUILayout.Button("Get Streaming Assets Path"))
            {
                GameDataHandler.DataHandler.DataPath = Application.streamingAssetsPath;
                serializedObject.Update();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}