using UnityEngine;

namespace PXE.Core.Tools.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PXE Settings", menuName = "PXE/Settings/PXE Settings", order = 99)]
    public class PXESettingsObject : ScriptableObject
    {
        [field: SerializeField] public virtual ProjectSettingsObject CurrentProjectSettings { get; set; }
    }
}