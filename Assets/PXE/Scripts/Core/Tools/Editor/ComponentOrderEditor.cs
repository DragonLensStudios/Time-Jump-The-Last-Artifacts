using UnityEditor;
using UnityEngine;

namespace PXE.Scripts.Core.Tools.Editor
{
    public class ComponentOrderEditor
    {
        [MenuItem("CONTEXT/Component/Move To Top")]
        public static void MoveToTop(MenuCommand menuCommand)
        {
            var component = menuCommand.context as Component;
            if (!component) return;
            foreach (var selected in Selection.gameObjects)
            {
                var comp = selected.GetComponent(component.GetType());
                if (!comp || comp is Transform) continue;
                while (UnityEditorInternal.ComponentUtility.MoveComponentUp(comp)) {};
            }
        }

        [MenuItem("CONTEXT/Component/Move To Bottom")]
        public static void MoveToBottom(MenuCommand menuCommand)
        {
            var component = menuCommand.context as Component;
            if (!component) return;
            foreach (var selected in Selection.gameObjects)
            {
                var comp = selected.GetComponent(component.GetType());
                if (!comp) continue;
                while (UnityEditorInternal.ComponentUtility.MoveComponentDown(comp)) {};
            }
        }
    }
}