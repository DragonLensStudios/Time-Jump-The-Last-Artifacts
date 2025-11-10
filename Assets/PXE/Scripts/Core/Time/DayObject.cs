using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Time
{
    /// <summary>
    ///  Represents the DayObject.
    /// </summary>
    [CreateAssetMenu(fileName ="Day", menuName ="PXE/Game/Time/Day", order = 2)]
    public class DayObject : ScriptableObjectController
    {
        [field: Tooltip("The abbreviated name with 1 letter of the DayObject")]
        [field: SerializeField] public virtual string Abbreviated1LetterName { get; set; }
        
        [field: Tooltip("The abbreviated name with 2 letters of the DayObject")]
        [field: SerializeField] public virtual string Abbreviated2LetterName { get; set; }
        
        [field: Tooltip("The abbreviated name with 3 letters of the DayObject")]
        [field: SerializeField] public virtual string Abbreviated3LetterName { get; set; }
        
        [field: Tooltip("The value of the DayObject")]
        [field: SerializeField] public virtual int Value { get; set; }
        
        [field: Tooltip("The hours in the DayObject")]
        [field: SerializeField] public virtual int HoursInDay { get; set; } = 24;
    }
}