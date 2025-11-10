using System;
using PXE.Core.Editor.Objects;
using UnityEditor;

namespace PXE._TESTING.Editor
{
    [CustomEditor(typeof(MyTesterManager))]
    public class MyTesterManagerEditor : ObjectControllerEditor
    {
        protected MyTesterManager manager;

        private void OnEnable()
        {
            manager = (MyTesterManager)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawObjectControllerInspector();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}