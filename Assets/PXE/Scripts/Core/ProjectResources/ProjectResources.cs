using PXE.Core.Settings;
using UnityEngine;

namespace PXE.Core.ProjectResources
{
    //TODO: Change all projects to use the ProjectResources and setup folder sctructure to match
    public static class ProjectResources
    {
        public static T Load<T>(string path) where T : Object
        {
            var r = Resources.Load<T>(string.IsNullOrWhiteSpace(PXESettings.ResourcesSubFolderFilter) ?
                path : $"{PXESettings.ResourcesSubFolderFilter}/{path}");
            return r;
        }
        
        public static T[] LoadAll<T>(string path) where T : Object
        {
            var r = Resources.LoadAll<T>(string.IsNullOrWhiteSpace(PXESettings.ResourcesSubFolderFilter) ?
                path : $"{PXESettings.ResourcesSubFolderFilter}/{path}");
            return r;
        }
    }
}