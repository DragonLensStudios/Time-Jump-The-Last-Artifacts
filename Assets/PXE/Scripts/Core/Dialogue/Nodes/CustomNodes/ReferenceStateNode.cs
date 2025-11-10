using PXE.Core.Dialogue.xNode.Scripts;
using PXE.Core.SerializableTypes;

namespace PXE.Core.Dialogue.Nodes.CustomNodes
{
    /// <summary>
    /// Represents a reference state node in a dialogue graph.
    /// </summary>
    public class ReferenceStateNode : BaseNode
    {
        [Input] public Connection Input;
        [Output] public Connection ExitTrue;
        public string ReferenceState;
        public bool OverrideSourceID = false;
        public bool OverrideTargetID = false;
        public SerializableGuid OverrideSourceIDValue;
        public SerializableGuid OverrideTargetIDValue;

        /// <summary>
        /// Returns the value of the specified output port.
        /// </summary>
        /// <param name="port">The output port to retrieve the value from.</param>
        /// <returns>The value of the output port.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}