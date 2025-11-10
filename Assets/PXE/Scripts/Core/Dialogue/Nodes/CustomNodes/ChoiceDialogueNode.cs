using PXE.Core.Dialogue.xNode.Scripts;
using UnityEngine;

namespace PXE.Core.Dialogue.Nodes.CustomNodes
{
    /// <summary>
    /// Represents a choice dialogue node in a dialogue graph.
    /// </summary>
    public class ChoiceDialogueNode : BaseNode
    {
        [Input] public Connection Input;
        [Output(dynamicPortList = true)] public string[] Answers;
        public string ActorName;
        public Sprite Portrait;
        [TextArea] public string DialogueText;
        public bool UseSourceActor;
        public bool UseTargetActor;
        public bool Manual;
        public bool OverridePortrait;

        


        /// <summary>
        /// Returns the value of the specified output port.
        /// </summary>
        /// <param name="port">The output port to retrieve the value from.</param>
        /// <returns>The value of the output port.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }

        /// <summary>
        /// Returns the sprite associated with the choice dialogue node.
        /// </summary>
        /// <returns>The sprite associated with the choice dialogue node.</returns>
        public override Sprite GetPortrait()
        {
            return Portrait;
        }

    }
}
