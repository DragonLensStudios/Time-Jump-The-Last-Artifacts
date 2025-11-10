using UnityEngine;

namespace PXE.Core.Tools.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Project Settings", menuName = "PXE/Settings/Project Settings", order = 1)]
    public class ProjectSettingsObject : ScriptableObject
    { 
        public static string DefaultName => "New Project";
        public static string DefaultAbbreviation => "NEW";
        public static string DefaultVersion => "0.1.0";
        
        [SerializeField] protected string _projectName = DefaultName;
        [SerializeField] protected string _projectAbbreviation = DefaultAbbreviation;

        public virtual string ProjectName
        {
            get => string.IsNullOrWhiteSpace(_projectName) ? DefaultName :  _projectName;
            set => _projectName = string.IsNullOrWhiteSpace(value) ? DefaultName : value;
        }

        public virtual string ProjectAbbreviation
        {
            get => string.IsNullOrWhiteSpace(_projectAbbreviation) ? DefaultAbbreviation :  _projectAbbreviation;
            set => _projectAbbreviation = string.IsNullOrWhiteSpace(value) ? DefaultAbbreviation : value;
        }

        [field: SerializeField] public virtual string Version { get; set; } = DefaultVersion;
        [field: SerializeField] public virtual string CompanyName { get; set; } = string.Empty;
        [field: SerializeField] public virtual string ResourcesFolderName { get; set; } = string.Empty;


        public virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_projectName))
            {
                _projectName = "New Project";
            }
            
            if (string.IsNullOrWhiteSpace(_projectAbbreviation))
            {
                _projectAbbreviation = "NEW";
            }
        }
    }
}