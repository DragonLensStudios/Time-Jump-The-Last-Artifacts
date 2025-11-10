using PXE.Core.Audio;
using PXE.Core.Dialogue.xNode.Scripts;
using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Core.Dialogue.Nodes.CustomNodes
{
    public class AudioNode : BaseNode
    {
        [Input] public Connection Input;
        [Output] public Connection ExitTrue;
        [Output] public Connection ExitFalse;
        public AudioObject Audio;
        public AudioChannel AudioChannel;
        public AudioOperation AudioOperation;
        public bool UseRandomVolume = false;
        public bool UseRandomPitch = false;
        [Range(0,1)] public float AudioVolume = 1;
        [Range(-3f,3f)] public float AudioPitch = 1;

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