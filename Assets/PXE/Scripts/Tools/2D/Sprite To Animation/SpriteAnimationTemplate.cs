using System.Collections.Generic;
using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Tools._2D.Sprite_To_Animation
{
    // TODO: Change the fields to be properties with backing fields and add tooltips
    /// <summary>
    ///  Represents the SpriteAnimationTemplate.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTemplate", menuName = "PXE/Sprite Animation Template")]
    public class SpriteAnimationTemplate : ScriptableObjectController
    {
        public string templateName;
        public Vector2Int gridSize;  // Grid size for slicing
        public List<AnimationDetail> animationDetails;  // Animation details per grid cell or row
    }
}