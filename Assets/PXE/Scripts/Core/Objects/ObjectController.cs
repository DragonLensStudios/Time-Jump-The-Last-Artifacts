using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Extensions.ObjectExtensions;
using PXE.Core.Interfaces;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.SerializableTypes;
using UnityEngine;
using UnityEngine.UI;

//TODO: Fix Object state when changing and make sure that children propagate the state correctly
namespace PXE.Core.Objects
{
    /// <summary>
    ///  Represents the ObjectController.
    /// </summary>
    public class ObjectController : MonoBehaviour, IGameObject
    {
        [field: Tooltip("The Name of the object.")]
        [field: SerializeField,HideInInspector]
        public virtual string Name { get; set; }

        [field: Tooltip("The ID of the object.")]
        [field: SerializeField, HideInInspector]
        public virtual SerializableGuid ID { get; set; }

        [field: Tooltip("When Enabled, The ID will be able to be manually set.")]
        [field: SerializeField,HideInInspector]
        public virtual bool IsManualID { get; set; } = false;

        [field: SerializeField,HideInInspector] public virtual bool IsInitialized { get; set; }

        [SerializeField,HideInInspector] protected bool isActive = true;

        // [SerializeField] protected bool isControllerActive = true;
        // [SerializeField] protected InheritBoolType controllerActiveType = InheritBoolType.Inherit;
        [SerializeField, HideInInspector] protected ActiveType activeType = ActiveType.Inherit;
        [SerializeField, HideInInspector] protected ChildrenActiveType componentActiveType = ChildrenActiveType.EachInherit;

        /// <summary>
        ///  This method sets the active state of the object.
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                if (!gameObject.activeSelf) return false;
                switch (ActiveType)
                {
                    case ActiveType.Active:
                        return true;
                    case ActiveType.Inactive:
                        return false;
                    default:
                        if (transform.parent == null) return true;
                        var parentOC = transform.parent.GetComponent<ObjectController>();
                        return parentOC == null || parentOC.isActive;
                }
            }
            set
            {
                isActive = value;
                UpdateActive();
            }
        }

        public virtual ActiveType ActiveType
        {
            get => activeType;
            set
            {
                var oldActive = isActive;
                activeType = value;
                switch (value)
                {
                    case ActiveType.Active:
                        isActive = true;
                        break;
                    case ActiveType.Inactive:
                        isActive = false;
                        break;
                    default:
                        if (transform.parent == null)
                        {
                            isActive = true;
                            break;
                        }

                        var parentOC = transform.parent.GetComponent<ObjectController>();
                        isActive = parentOC == null || parentOC.isActive;
                        break;
                }

                if (oldActive != isActive)
                {
                    UpdateActive();
                }
            }
        }

        public virtual ChildrenActiveType ComponentActiveType
        {
            get => componentActiveType;
            set
            {
                var oldActiveType = componentActiveType;
                componentActiveType = value;
                if (oldActiveType != componentActiveType)
                {
                    UpdateActive();
                }
            }
        }

        public virtual void Awake()
        {
            if (!isActive) return;
            if (!IsInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        ///  This method calls the Enable method if the object is active and the Disable method if the object is not active and sets the game object name.
        /// </summary>
        public virtual void Start()
        {
            if (isActive)
            {
                OnActive();
            }
            else
            {
                OnInactive();
            }

            UpdateIdentity(gameObject);
        }

        /// <summary>
        ///  Registers the ObjectController for the TransformPositionMessage message.
        /// </summary>
        public virtual void OnActive()
        {
            MessageSystem.MessageManager.RegisterForChannel<TransformPositionMessage>(MessageChannels.Object, TransformPositionMessageHandler);
            // Add any additional logic for enabling here
        }

        /// <summary>
        ///  Unregisters the ObjectController for the TransformPositionMessage message.
        /// </summary>
        public virtual void OnInactive()
        {
            MessageSystem.MessageManager.UnregisterForChannel<TransformPositionMessage>(MessageChannels.Object, TransformPositionMessageHandler);
            // Add any additional logic for disabling here
        }

        public virtual void Update()
        {
            if (!isActive) return;
        }

        public virtual void LateUpdate()
        {
            if (!isActive) return;
        }

        public virtual void FixedUpdate()
        {
            if (!isActive) return;
        }

        /// <summary>
        ///  This method sets the active state of the object and propagates the active state to all child objects.
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetObjectActive(bool active)
        {
            ActiveType = active ? ActiveType.Active : ActiveType.Inactive;
        }

        public virtual void UpdateActive()
        {
            if (gameObject == null) return;

            gameObject.SetActive(true);
            bool active = isActive;

            // Update all components on this game object
            Component[] allComponents = gameObject.GetComponents<Component>();
            foreach (var comp in allComponents)
            {
                UpdateComponentActive(comp);
            }

            // Update active state for child ObjectController components
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent<ObjectController>(out var childController)) continue;

                // Propagate the active state based on the parent's ActiveType
                bool childActiveState = childController.isActive;

                switch (childController.ActiveType)
                {
                    case ActiveType.Active:
                        // Child's active state remains true
                        childActiveState = true;
                        break;
                    case ActiveType.Inactive:
                        // Child's active state remains false
                        childActiveState = false;
                        break;
                    default: // Inherit case
                        // Inherit the parent's isActive state
                        childActiveState = isActive;
                        break;
                }

                // Set the child's isActive state based on the calculated active state
                childController.IsActive = childActiveState;

                // Update the child's active state
                childController.UpdateActive();
            }

            switch (ActiveType)
            {
                case ActiveType.Active:
                    OnActive();
                    break;
                case ActiveType.Inactive:
                    OnInactive();
                    break;
                default:
                    if (isActive)
                    {
                        OnActive();
                    }
                    else
                    {
                        OnInactive();
                    }
                    break;
            }

            isActive = active;
            UpdateIdentity(gameObject);
        }
        
        /// <summary>
        ///  This method sets the game object name based on the active state and the object identity name.
        /// </summary>
        /// <returns></returns>
        public virtual string SetGameObjectName()
        {
            // Ensure the base Name does not contain the suffix
            Name = Name.Replace(" (Inactive)", "").Replace(" (Active)", "");
            
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = gameObject.name.Replace(" (Inactive)", "").Replace(" (Active)", "");
            }

            // Set the GameObject's name based on the base Name and active state
            if (isActive)
            {
                gameObject.name = Name + " (Active)";
            }
            else
            {
                gameObject.name = Name + " (Inactive)";
            }

            return gameObject.name;
        }

        /// <summary>
        ///  This method handles the TransformPositionMessage message and sets the position and rotation of the object.
        /// </summary>
        /// <param name="message"></param>
        public virtual void TransformPositionMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<TransformPositionMessage>().HasValue) return;
            var data = message.Message<TransformPositionMessage>().GetValueOrDefault();
            if (!data.ID.Equals(ID)) return;
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }

        /// <summary>
        ///  This method sets the active state of the object to false when the object is destroyed.
        /// </summary>
        public virtual void OnDestroy()
        {
            OnInactive();
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
        }


        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public static void UpdateIdentity(GameObject go, IEnumerable<GameObject> gos = null)
        {
//TODO: Fix prefabs with old naming.
// #if UNITY_EDITOR
//             string baseName = go.name.Replace(" (Disabled)", "").Replace(" (Enabled)", "");
//             go.name = baseName;
//             UnityEditor.EditorUtility.SetDirty(go);
// #endif
            
            
            if (!go.TryGetComponent<ObjectController>(out var primaryComp))
            {
                if (go.hideFlags.HasFlag(HideFlags.NotEditable) || go.hideFlags.HasFlag(HideFlags.HideAndDontSave)) return;
#if UNITY_EDITOR
                try
                {
                    primaryComp = UnityEditor.ObjectFactory.AddComponent<ObjectController>(go);
                }
                catch (Exception)
                {
                    return;
                }
#else
                primaryComp = go.AddComponent<ObjectController>();
#endif
            }

            if (primaryComp == null) return;
            List<ObjectController> ocs = new List<ObjectController>();

            primaryComp.SetGameObjectName();

            if (SerializableGuid.IsEmpty(primaryComp.ID))
            {
                primaryComp.ID = SerializableGuid.CreateNew;
            }
#if UNITY_EDITOR
            //TODO: Handle making prefabs with addressables and setting the addressable to the ID of the object.
            // UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry()
#endif
            var comps = go.GetComponents<ObjectController>();
            foreach (var comp in comps)
            {
                comp.ID = primaryComp.ID;
                comp.Name = primaryComp.Name;
                comp.IsManualID = primaryComp.IsManualID;
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(go);
#endif
        }

        public static void UpdateAllIdentities()
        {
            var gos = FindObjectsOfType<GameObject>().ToList();
            var ocs = GetExistingObjectControllers();
            foreach (var oc in ocs)
            {
                if (oc == null) continue;
                if (oc.gameObject == null) continue;
                if (gos.Contains(oc.gameObject)) continue;
                gos.Add(oc.gameObject);
            }

            foreach (var go in gos)
            {
                UpdateIdentity(go, gos);
            }

        }

        public virtual IEnumerable<T> GetExistingIDs<T>() where T : IID
        {
            return GetExistingObjectControllers() as IEnumerable<T>;
        }

        public virtual bool IsExistingID<T>(SerializableGuid id, IEnumerable<T> objs) where T : IID => objs.Any(o => o.ID.Equals(id));

        public static IEnumerable<ObjectController> GetExistingObjectControllers()
        {
#if UNITY_EDITOR
            var ocs = FindObjectsOfType<ObjectController>().ToList();
            var prefabUnityIds = UnityEditor.AssetDatabase.FindAssets("t:Prefab");
            foreach (var unityId in prefabUnityIds)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(unityId);
                var oc = UnityEditor.AssetDatabase.LoadAssetAtPath<ObjectController>(assetPath);
                if (oc == null) continue;
                if (ocs.Contains(oc)) continue;
                ocs.Add(oc);
            }
#else
            var ocs = FindObjectsOfType<ObjectController>();
#endif
            return ocs;
        }

        public virtual void UpdateComponentActive(Component comp)
        {
            var primaryComp = comp.GetComponent<ObjectController>();
            bool active;
            {
                if(primaryComp == null)
                {
                    active = comp.gameObject.activeSelf;
                }
                else
                {
                    comp.gameObject.SetActive(true);
                    active = primaryComp.isActive;
                }
            } 
            switch (comp)
            {
                case Transform transform:
                {
                    break;
                }
                case ObjectController controller:
                {
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            controller.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            controller.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            controller.enabled = active;
                            break;
                    }

                    break;
                }
                case Graphic graphic:
                {
                    // var graphicActive = active;
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            graphic.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            graphic.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            graphic.enabled = isActive;
                            break;
                    }

                    graphic.raycastTarget = graphic.enabled; // makes sure it doesn't receive input
                    graphic.CrossFadeAlpha(graphic.enabled ? 1 : 0, 0f, true); // 0f duration means instant

                    Selectable selectable = GetComponent<Selectable>(); // for buttons, sliders, etc.
                    if (selectable)
                    {
                        selectable.interactable = graphic.enabled;
                    }

                    if (!TryGetComponent<LayoutElement>(out var layoutElement))
                    {
                        layoutElement = gameObject.AddComponent<LayoutElement>();
                    }
                    
                    layoutElement.ignoreLayout = !graphic.enabled;
                    graphic.enabled = graphic.enabled;
                    break;
                }

                // For Sprite Renderers
                case SpriteRenderer spriteRenderer:
                {
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            spriteRenderer.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            spriteRenderer.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            spriteRenderer.enabled = isActive;
                            break;  
                    }

                    break;
                }
                // For Renderers
                case Renderer rend:
                {
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            rend.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            rend.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            rend.enabled = isActive;
                            break;
                    }

                    break;
                }
                // For Colliders
                case Collider col:
                {
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            col.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            col.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            col.enabled = isActive;
                            break;
                    }

                    break;
                }
                case Camera cam:
                {
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            cam.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            cam.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            cam.enabled = isActive;
                            break;
                    }

                    break;
                }
                case CanvasRenderer canvasRenderer:
                {
                    break;
                }
                // Default all components that can be enabled/disabled
                case Behaviour behaviour:
                {
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            behaviour.enabled = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            behaviour.enabled = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            break;
                        default:
                            behaviour.enabled = active;
                            break;
                    }

                    break;
                }
                default:
                    // Determine the new active state based on BehaviorActiveType
                    bool newActiveState = comp.TryGet<bool>("enabled");
                    switch (ComponentActiveType)
                    {
                        case ChildrenActiveType.EachActive:
                            newActiveState = true;
                            break;
                        case ChildrenActiveType.EachInactive:
                            newActiveState = false;
                            break;
                        case ChildrenActiveType.EachManual:
                            return;
                        default:
                            newActiveState = isActive;
                            break;
                    }

                    if (!comp.TrySet("enabled", newActiveState))
                    {
                        Debug.LogWarning($"{comp.GetType().FullName}: component does not have an enabled property.");
                    }

                    break;
            }
        }

        /// <summary>
        ///  This method resets the object's name and ID.
        /// </summary>
        public virtual void Reset()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = gameObject.name;
            }

            if (SerializableGuid.IsEmpty(ID))
            {
                ID = SerializableGuid.CreateNew;
            }
        }
    }
}
