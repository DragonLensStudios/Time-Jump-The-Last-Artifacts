using System;
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Extensions.GameObjectExtensions;
using PXE.Core.Interfaces;
using PXE.Core.Levels.Managers;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using PXE.Core.Spawning.Messaging.Messages;
using PXE.Core.Tilemap;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace PXE.Core.Spawning.Managers
{
    /// <summary>
    /// Represents the SpawnManager.
    /// The SpawnManager class provides functionality related to spawnmanager management.
    /// This class contains methods and properties that assist in managing and processing spawnmanager related tasks.
    /// </summary>
    public class SpawnManager : ObjectController
    {
        /// <summary>
        /// Singleton instance of the SpawnManager.
        /// </summary>
        public static SpawnManager Instance { get; private set; }
    
        [field: Tooltip("When UseSpawningBounds is true this will show a gizmo in the editor with this color.")]
        [field: SerializeField] public Color BoundsGizmoColor { get; set; } = Color.green;
        
        [field: Tooltip("Single object to spawn")]
        [field: SerializeField] public ObjectController ObjectToSpawn { get; set; }
        
        [field: Tooltip("List of game objects to spawn.")]
        [field: SerializeField] public List<ObjectController> ObjectsToSpawn { get; set; }
        
        [field: Tooltip("If true, objects are chosen randomly from the ObjectsToSpawn list for spawning.")]
        [field: SerializeField] public bool UseRandomSpawnObjects { get; set; }
        
        [field: Tooltip("Minimum amount of objects to spawn.")]
        [field: SerializeField] public int AmountToSpawnMin { get; set; }
        
        [field: Tooltip("Maximum amount of objects to spawn.")]
        [field: SerializeField] public int AmountToSpawnMax { get; set; }
        
        [field: Tooltip("When true will spawn objects inside the bounds defined.")]
        [field: SerializeField] public bool UseSpawningBounds { get; set; }
        
        [field: Tooltip("This the the min bounds for spawning objects")]
        [field: SerializeField] public Vector3 MinBounds { get; set; }
        
        [field: Tooltip("This the the max bounds for spawning objects")]
        [field: SerializeField] public Vector3 MaxBounds { get; set; }
        
        [field: Tooltip("Radius for object detection around spawn points.")]
        [field: SerializeField] public Vector3 ObjectDetectRadius { get; set; }
        
        [field: Tooltip("The offset for object spawning")]
        [field: SerializeField] public Vector3 ObjectSpawnOffset { get; set; }
        
        [field: Tooltip("When true it will use the Tilemap found with the Tilemap To Detect ID as the Tilemap to check for available spawn locations")]
        [field: SerializeField] public bool UseTilemapToPopulateAvailableSpawnLocations { get; set; }
        
        [field: Tooltip("ID used to find Tilemap that us used for finding available spawn points.")]
        [field: SerializeField] public SerializableGuid TilemapToDetectID { get; set; }
        
        [field: Tooltip("Tilemap to use for detecting available spawn points.")]
        [field: SerializeField] public UnityEngine.Tilemaps.Tilemap TilemapToDetectAvailableSpawnPoints { get; set; }
        
        [field: Tooltip("ID used to find Tilemap that us used as the parent for spawning")]
        [field: SerializeField] public SerializableGuid TilemapToSpawnInsideID { get; set; }
        
        [field: Tooltip("Tilemap to use for the spawn parent")]
        [field: SerializeField] public UnityEngine.Tilemaps.Tilemap TilemapToSpawnInside { get; set; }
        
        [field: Tooltip("Use object pooling for efficiency.")]
        [field: SerializeField] public bool UseObjectPool { get; set; }
        
        [field: Tooltip("Minimum capacity of the object pool.")]
        [field: SerializeField] public int ObjectPoolMinCapacity { get; set; }
        
        [field: Tooltip("Maximum capacity of the object pool.")]
        [field: SerializeField] public int ObjectPoolMaxCapacity { get; set; }
        
        [field: Tooltip("List of available locations for spawning objects.")]
        [field: SerializeField] public List<Vector3> AvailableSpawnLocations  { get; set; }
        
        [field: Tooltip("Storage of currently active spawned objects.")]
        [field: SerializeField] public List<ObjectList> ActiveObjects { get; set; }
        
        [field: Tooltip("Pool of game objects for efficient spawning/despawning.")]
        [field: SerializeField] public ObjectPool<ObjectController> Pool { get; set; }
        
        [field: Tooltip("Queue of game objects to be spawned.")]
        [field: SerializeField] public Queue<ObjectController> SpawnObjectsQueue { get; set; }= new ();
        
        
        /// <summary>
        ///  Singleton pattern for the spawn manager.
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
        }
        
        /// <summary>
        ///  This method registers the SpawnManager for the SpawnMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<SpawnMessage>(MessageChannels.Spawning, HandleSpawnMessage);

        }

        /// <summary>
        ///  This method unregisters the SpawnManager for the SpawnMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<SpawnMessage>(MessageChannels.Spawning, HandleSpawnMessage);

        }

        /// <summary>
        /// Sets up the object pool and queue for spawning objects on game start.
        /// </summary>
        public override void Start()
        {
            base.Start();
            SetupPool();
            SpawnObjectsQueue.Clear();
            for (int i = 0; i < ObjectsToSpawn.Count; i++)
            {
                if (!SpawnObjectsQueue.Contains(ObjectsToSpawn[i]))
                {
                    SpawnObjectsQueue.Enqueue(ObjectsToSpawn[i]);
                }
            }

            var tileMapController = FindFirstObjectByType<TilemapController>(FindObjectsInactive.Include);
            if (tileMapController == null) return;
        
            if (tileMapController.Tilemaps.TryGetValue(TilemapToSpawnInsideID, out var parentTilemap))
            {
                TilemapToSpawnInside = parentTilemap;
            }
            if (!UseTilemapToPopulateAvailableSpawnLocations) return;
            if(tileMapController.Tilemaps.TryGetValue(TilemapToDetectID, out var detectTilemap))
            {
                TilemapToDetectAvailableSpawnPoints = detectTilemap;
            }
        }
    
        /// <summary>
        /// Initializes the object pool if UseObjectPool is set to true.
        /// </summary>
        public virtual void SetupPool()
        {
            if (UseObjectPool)
            {
                Pool = new ObjectPool<ObjectController>(OnPoolObjectCreate, OnPoolObjectGet, OnPoolObjectRelease, OnPoolObjectDestroy, false, ObjectPoolMinCapacity, ObjectPoolMaxCapacity);
            }
        }
    
        /// <summary>
        /// Creates an object to be added to the pool.
        /// </summary>
        public virtual ObjectController OnPoolObjectCreate()
        {
            Vector3 randomPos = Vector3.zero;
            ObjectController objectToSpawn = ObjectToSpawn;
        
            if(AvailableSpawnLocations.Count > 0)
            {
                randomPos = AvailableSpawnLocations[Random.Range(0, AvailableSpawnLocations.Count)];
            }

            if (SpawnObjectsQueue.Count > 0 && ObjectsToSpawn.Count > 0)
            {
                objectToSpawn = Instantiate(UseRandomSpawnObjects ? ObjectsToSpawn[Random.Range(0, ObjectsToSpawn.Count)] : SpawnObjectsQueue.Dequeue(), randomPos, Quaternion.identity);
            }
            else if(ObjectToSpawn != null)
            {
                objectToSpawn = Instantiate(objectToSpawn, randomPos, Quaternion.identity);
            }
        
            objectToSpawn.transform.parent = transform;
            AvailableSpawnLocations.Remove(randomPos);
            
            var objList = new ObjectList
            {
                ID = gameObject.GetObjectID(),
                LevelID = LevelManager.Instance.CurrentLevelObject.ID,
                Name = LevelManager.Instance.CurrentLevelObject.Name,
                ObjectControllers = new List<ObjectController>()
            };
            

            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevelObject.ID));
            if (levelActiveObjects == null && ActiveObjects.Count <= 0)
            {
                ActiveObjects.Add(objList);
                var activeObject = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevelObject.ID));
                if (activeObject != null)
                {
                    activeObject.ObjectControllers.Add(objectToSpawn);
                }

                levelActiveObjects = objList;
            }

            if (levelActiveObjects == null) return objectToSpawn;
            if (!levelActiveObjects.ID.Equals(objList.ID))
            {
                levelActiveObjects.ObjectControllers.Add(objectToSpawn);
            }

            return objectToSpawn;
        }
    
        /// <summary>
        /// Prepares the object for use from the pool.
        /// </summary>
        public virtual void OnPoolObjectGet(ObjectController obj)
        {

            if(AvailableSpawnLocations.Count > 0)
            {
                obj.transform.position = AvailableSpawnLocations[Random.Range(0, AvailableSpawnLocations.Count)];
            }
            
            obj.SetObjectActive(true);
        }
    
        /// <summary>
        /// Prepares the object to be returned to the pool.
        /// </summary>
        public virtual void OnPoolObjectRelease(ObjectController obj)
        {
            obj.SetObjectActive(false);
        }
    
        /// <summary>
        /// Destroys an object when it is removed from the pool.
        /// </summary>
        public virtual void OnPoolObjectDestroy(ObjectController obj)
        {
            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.ID.Guid == LevelManager.Instance.CurrentLevelObject.ID.Guid);
            if (levelActiveObjects == null) return;
            if (levelActiveObjects.ObjectControllers.Contains(obj))
            {
                levelActiveObjects.ObjectControllers.Remove(obj);
            }

            Destroy(obj);
        }
    
        /// <summary>
        /// Despawns all active objects.
        /// Executes the DespawnObjects method.
        /// Handles the DespawnObjects functionality.
        /// </summary>
        public virtual void DespawnObjects()
        {
            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.ID.Guid == LevelManager.Instance.CurrentLevelObject.ID.Guid);
            if (levelActiveObjects == null) return;

            for (int i = 0; i < levelActiveObjects.ObjectControllers.Count; i++)
            {
                var obj = levelActiveObjects.ObjectControllers[i];
                DespawnOject(obj);
            }
        }

        /// <summary>
        /// Spawns a specific object at a location from the available spawn locations.
        /// Executes the SpawnObject method.
        /// Handles the SpawnObject functionality.
        /// </summary>
        public virtual void SpawnObject(ObjectController obj, params Vector3[] spawnLocations)
        {
            AddSpawnLocations(spawnLocations);
        
            if (AvailableSpawnLocations.Count <= 0) return;
        
            var randomPos = AvailableSpawnLocations[Random.Range(0, AvailableSpawnLocations.Count)];

            obj.gameObject.SetObjectID(new SerializableGuid(Guid.NewGuid()));
            var objList = new ObjectList
            {
                ID = gameObject.GetObjectID(),
                Name = LevelManager.Instance.CurrentLevelObject.Name,
                LevelID = LevelManager.Instance.CurrentLevelObject.ID,
                ObjectControllers = new List<ObjectController>()
            };
            
            if (UseObjectPool)
            {
                var poolGo = Pool.Get();
                poolGo.transform.position = randomPos;
                poolGo.transform.parent = TilemapToSpawnInside != null ? TilemapToSpawnInside.transform : transform;
                
                if (ActiveObjects is { Count: 0 })
                {
                    ActiveObjects.Add(objList);
                }
                else
                {
                    if (!ActiveObjects.Exists(x=> x.LevelID.Equals(objList.LevelID)))
                    {
                        ActiveObjects.Add(objList);
                    }
                }
                
                var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevelObject.ID));
                if (levelActiveObjects == null) return;
                if (!levelActiveObjects.ObjectControllers.Contains(poolGo))
                {
                    levelActiveObjects.ObjectControllers.Add(poolGo);
                }
                if (!SpawnObjectsQueue.Contains(poolGo))
                {
                    SpawnObjectsQueue.Enqueue(poolGo);
                }
                AvailableSpawnLocations.Remove(randomPos);
            }
            else
            {
                var spawnedObject = Instantiate(obj, randomPos, Quaternion.identity);
                var spawnedID = spawnedObject.GetComponent<IID>();
                spawnedObject.SetObjectActive(true);
                spawnedObject.transform.parent = TilemapToSpawnInside != null ? TilemapToSpawnInside.transform : transform;
                
                var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevelObject.ID));

                if (levelActiveObjects == null)
                {
                    levelActiveObjects = objList;
                    if (!levelActiveObjects.ID.Equals(spawnedID.ID))
                    {
                        levelActiveObjects.ObjectControllers.Add(spawnedObject);
                    }
                    ActiveObjects.Add(levelActiveObjects);
                }
                else
                {
                    if (!levelActiveObjects.ObjectControllers.Contains(spawnedObject))
                    {
                        levelActiveObjects.ObjectControllers.Add(spawnedObject);
                    }
                }

                if (!SpawnObjectsQueue.Contains(spawnedObject))
                {
                    SpawnObjectsQueue.Enqueue(spawnedObject);
                }
                AvailableSpawnLocations.Remove(randomPos);
            }
        }
    
        /// <summary>
        /// Despawns a specific object.
        /// Executes the DespawnOject method.
        /// Handles the DespawnOject functionality.
        /// </summary>
        public virtual void DespawnOject(ObjectController obj)
        {
            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.ID.Guid == LevelManager.Instance.CurrentLevelObject.ID.Guid);
            if (levelActiveObjects == null) return;
            var existingActiveObject = levelActiveObjects.ObjectControllers.FirstOrDefault(x => x == obj);
            if(existingActiveObject == null) return;
            if(!existingActiveObject.IsActive) return;
            levelActiveObjects.ObjectControllers.Remove(existingActiveObject);
            
            if (UseObjectPool)
            {
                Pool.Release(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    
        /// <summary>
        /// Spawns a random amount of objects within the min and max range at available spawn locations.
        /// Executes the SpawnObjects method.
        /// Handles the SpawnObjects functionality.
        /// </summary>
        public virtual void SpawnObjects(params Vector3[] spawnPositions)
        {
            var amountToSpawn = Random.Range(AmountToSpawnMin, AmountToSpawnMax + 1);
            for (int i = 0; i < amountToSpawn; i++)
            {
                ObjectController obj = null;
                obj = UseRandomSpawnObjects ? ObjectsToSpawn[Random.Range(0, ObjectsToSpawn.Count)] : SpawnObjectsQueue.Dequeue();
                SpawnObject(obj, spawnPositions);
            }
        }
    
        /// <summary>
        /// Adds new spawn locations for objects.
        /// Executes the AddSpawnLocations method.
        /// Handles the AddSpawnLocations functionality.
        /// </summary>
        public virtual void AddSpawnLocations(params Vector3[] spawnPositions)
        {
            AvailableSpawnLocations.Clear();
            if (spawnPositions is not { Length: 0 })
            {
                foreach (var pos in spawnPositions)
                {
                    if (UseSpawningBounds)
                    {
                        if (pos.x < MinBounds.x || pos.x > MaxBounds.x || pos.y < MinBounds.y || pos.y > MaxBounds.y)
                        {
                            continue;
                        }
                    }
                    Vector3Int localPlace = new Vector3Int((int)pos.x, (int)pos.y , (int)pos.z);
                    Vector3 place = TilemapToDetectAvailableSpawnPoints.CellToWorld(localPlace) + ObjectSpawnOffset;
                    if (TilemapToDetectAvailableSpawnPoints.HasTile(localPlace))
                    {
                        var objHere = GetObjectsAtPosition(place, ObjectDetectRadius, 0f);
                        if (!objHere.objectFound)
                        {
                            AvailableSpawnLocations.Add(place);
                        }
                        else
                        {
                            for (int i = 0; i < objHere.colliders.Count; i++)
                            {
                                var obj = objHere.colliders[i];
                                if (obj != null)
                                {
                                    Debug.Log($"Object: {obj.name} at X:{place.x} Y:{place.y} Z:{place.z}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var pos in TilemapToDetectAvailableSpawnPoints.cellBounds.allPositionsWithin)
                {
                    if (UseSpawningBounds)
                    {
                        if (pos.x < MinBounds.x || pos.x > MaxBounds.x || pos.y < MinBounds.y || pos.y > MaxBounds.y)
                        {
                            continue;
                        }
                    }
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    Vector3 place = TilemapToDetectAvailableSpawnPoints.CellToWorld(localPlace) + ObjectSpawnOffset;
                    if (TilemapToDetectAvailableSpawnPoints.HasTile(localPlace))
                    {
                        var objHere = GetObjectsAtPosition(place, ObjectDetectRadius, 0f);
                        if (!objHere.objectFound)
                        {
                            AvailableSpawnLocations.Add(place);
                        }
                        else
                        {
                            for (int i = 0; i < objHere.colliders.Count; i++)
                            {
                                var obj = objHere.colliders[i];
                                if (obj != null)
                                {
                                    Debug.Log($"Object: {obj.name} at X:{place.x} Y:{place.y} Z:{place.z}");
                                }
                            }
                        }
                    }
                }
            }
        }
    
        /// <summary>
        /// Detects whether there are objects at the given position within the given radius.
        /// </summary>
        public virtual (bool objectFound, List<Collider2D> colliders) GetObjectsAtPosition(Vector2 position, Vector2 radius, float angle)
        {
            Collider2D[] intersecting = Physics2D.OverlapBoxAll(position, radius, angle);
            return (intersecting.Length != 0, intersecting.ToList());
        }
    
        /// <summary>
        /// Handles SpawnMessage and performs spawn/despawn operations based on message data.
        /// </summary>
        public virtual void HandleSpawnMessage(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<SpawnMessage>().HasValue) return;
            var data = message.Message<SpawnMessage>().GetValueOrDefault();
            if (data.ObjectToSpawn != null)
            {
                ObjectToSpawn = data.ObjectToSpawn;
                SpawnObjectsQueue.Clear();
                SpawnObjectsQueue.Enqueue(ObjectToSpawn);
            }
            if (data.SpawnGameObjects is not {Count: 0})
            {
                ObjectsToSpawn = data.SpawnGameObjects;
                SpawnObjectsQueue.Clear();
                for (int i = 0; i < ObjectsToSpawn.Count; i++)
                {
                    SpawnObjectsQueue.Enqueue(ObjectsToSpawn[i]);
                }
            }
        
            if (data.AmountToSpawnMin.HasValue)
            {
                AmountToSpawnMin = data.AmountToSpawnMin.Value;
            }

            if (data.AmountToSpawnMax.HasValue)
            {
                AmountToSpawnMax = data.AmountToSpawnMax.Value;
            }

            if (data.UseSpawningBounds.HasValue)
            {
                UseSpawningBounds = data.UseSpawningBounds.Value;
            }

            if (data.MinBounds.HasValue)
            {
                MinBounds = data.MinBounds.Value;
            }
            if (data.MaxBounds.HasValue)
            {
                MaxBounds = data.MaxBounds.Value;
            }

            if (data.ObjectDetectRadius.HasValue)
            {
                ObjectDetectRadius = data.ObjectDetectRadius.Value;
            }

            if (data.ObjectSpawnOffset.HasValue)
            {
                ObjectSpawnOffset = data.ObjectSpawnOffset.Value;
            }

            if (data.UseTilemapToPopulateAvailableSpawnLocations.HasValue)
            {
                UseTilemapToPopulateAvailableSpawnLocations = data.UseTilemapToPopulateAvailableSpawnLocations.Value;
            }
        
            if (data.UseObjectPool.HasValue)
            {
                UseObjectPool = data.UseObjectPool.Value;
            }

            if (data.ObjectPoolMinCapacity.HasValue)
            {
                ObjectPoolMinCapacity = data.ObjectPoolMinCapacity.Value;
            }

            if (data.ObjectPoolMaxCapacity.HasValue)
            {
                ObjectPoolMaxCapacity = data.ObjectPoolMaxCapacity.Value;
            }

            if (data.UseRandomSpawnObjects.HasValue)
            {
                UseRandomSpawnObjects = data.UseRandomSpawnObjects.Value;
            }

            if (data.SpawnPositions?.Count > 0)
            {
                AddSpawnLocations(data.SpawnPositions?.ToArray());
            }
            else
            {
                AddSpawnLocations();
            }
            if (data.TileMapToDetectID != null)
            {
                TilemapToDetectID = data.TileMapToDetectID;
            }

            if (data.TilemapToSpawnInsideID != null)
            {
                TilemapToSpawnInsideID = data.TilemapToSpawnInsideID;
            }

            switch (data.SpawnOperation)
            {
                case SpawnOperation.SpawnSingle:
                    var spawnGo = data.ObjectToSpawn;
                    if (spawnGo != null)
                    {
                        if (data.SpawnPositions?.Count > 0)
                        {
                            SpawnObject(spawnGo, data.SpawnPositions.ToArray());
                        }
                        else
                        {
                            SpawnObject(spawnGo);
                        }
                    }
                    break;
                case SpawnOperation.SpawnAll:
                    if (data.SpawnPositions?.Count > 0)
                    {
                        SpawnObjects(data.SpawnPositions.ToArray());
                    }
                    else
                    {
                        SpawnObjects();
                    }
                    break;
                case SpawnOperation.DespawnSingle:
                    var despawnGo = data.ObjectToSpawn;
                    if (despawnGo != null)
                    {
                        DespawnOject(despawnGo);
                    }
                    break;
                case SpawnOperation.DespawnAll:
                    DespawnObjects();
                    break;
            }
        }
        
        /// <summary>
        ///  Draws a gizmo in the editor for the spawning bounds.
        /// </summary>
        public virtual void OnDrawGizmos()
        {
            if (!UseSpawningBounds) return;
            Gizmos.color = BoundsGizmoColor;
            Vector3 center = (MinBounds + MaxBounds) * 0.5f;
            Vector3 size = MaxBounds - MinBounds;
            Gizmos.DrawCube(center, size);
        }

    }
}
