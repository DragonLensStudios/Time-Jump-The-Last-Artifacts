using System.Collections.Generic;
using System.Linq;
using PXE.Core.Levels;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.DEBUG.UI
{
    public class LevelSelectUiController : ObjectController
    {
        [field: Tooltip("The list of levels.")]
        [field: SerializeField] public List<LevelObject> Levels { get; set; }
        
        [field: Tooltip("The level select UI prefab.")]
        [field: SerializeField] public LevelSelectUiContainer LevelSelectUiPrefab { get; set; }
        
        [field: Tooltip("The level select content container.")]
        [field: SerializeField] public ObjectController LevelSelectContentContainer { get; set; }


        /// <summary>
        ///  Executes the LevelSelectUiController method and loads the levels.
        /// </summary>
        public override void Start()
        {
            base.Start();
            if (Levels.Count == 0 && DebugManager.Instance.Levels.Count == 0)
            {
                Levels = Resources.LoadAll<LevelObject>("Levels").ToList();
            }
            else
            {
                Levels = DebugManager.Instance.Levels;
            }
            ClearAndPopulateLevelSelect();
        }
        
        /// <summary>
        ///  Executes the LevelSelectUiController method and clears and populates the level select.
        /// </summary>
        public virtual void ClearAndPopulateLevelSelect()
        {
            for (int i = LevelSelectContentContainer.transform.childCount - 1; i >= 0; i--)
            {
                var child = LevelSelectContentContainer.transform.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var level in Levels)
            {
                var spawnedLevel = Instantiate(LevelSelectUiPrefab, LevelSelectContentContainer.transform);
                spawnedLevel.SetLevel(level);
            }
        }
    
    }
}
