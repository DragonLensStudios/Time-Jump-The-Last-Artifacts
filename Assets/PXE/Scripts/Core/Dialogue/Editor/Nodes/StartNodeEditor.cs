using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    /// <summary>
    /// Custom editor for the StartNode node.
    /// </summary>
    [NodeEditor.CustomNodeEditor(typeof(StartNode))]
    public class StartNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the StartNode node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as StartNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Exit)));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
