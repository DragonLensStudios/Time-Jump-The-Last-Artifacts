using PXE.Core.Objects;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PXE.Core.Lighting.Managers
{
    public class LightingManager : ObjectController
    {
        public static LightingManager Instance { get; private set; }
        
        [field: Tooltip("The global light.")]
        [field : SerializeField] public virtual Light2D GlobalLight { get; set; }
        
        [field: Tooltip("The global light intensity min.")]
        [field: SerializeField] public virtual float GlobalLightIntensityMin { get; set; } = 0.1f;
        
        [field: Tooltip("The global light intensity max.")]
        [field: SerializeField] public virtual float GlobalLightIntensityMax { get; set; } = 1f;

        /// <summary>
        ///  Singleton pattern for the lighting manager and sets the global light.
        /// </summary>
        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            base.Awake();
            GlobalLight = GetComponent<Light2D>();
        }
    }
    
}
