using PXE.Core.Dialogue.Editor.xNode.Scripts;
using PXE.Core.Dialogue.Nodes;

namespace PXE.Scripts.Core.Dialogue.Editor.Nodes
{
    /// <summary>
    /// Custom editor for the ExitNode_NoLoop_toStart node.
    /// </summary>
    [NodeEditor.CustomNodeEditor(typeof(ExitNode_NoLoop_toStart))]
    public class ExitNode_NoLoop_ToStartNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the ExitNode_NoLoop_toStart node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as ExitNode_NoLoop_toStart;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Entry)));
                NodeEditorGUILayout.PortField(segment.GetPort(nameof(segment.Exit)));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
