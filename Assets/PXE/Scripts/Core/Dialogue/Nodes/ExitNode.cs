using PXE.Core.Dialogue.xNode.Scripts;

namespace PXE.Core.Dialogue.Nodes
{
    /// <summary>
    /// Represents a node that serves as an exit point in the dialogue graph.
    /// </summary>
    public class ExitNode : BaseNode
    {
        [Input] public Connection Entry; // Input connection for the node.

        /// <summary>
        /// Retrieves the value of the node.
        /// </summary>
        /// <param name="port">The port of the node.</param>
        /// <returns>Always returns null.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}