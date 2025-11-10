using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using PXE.Core.Spawning.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Spawning
{
/// <summary>
/// Represents the SpawningTrigger.
/// The SpawningTrigger class provides functionality related to spawningtrigger management.
/// This class contains methods and properties that assist in managing and processing spawningtrigger related tasks.
/// </summary>
    public class SpawningTrigger : ObjectController
    {
        [field: Tooltip("Single object to spawn")]
        [field: SerializeField] public virtual ObjectController SpawnObject { get; set; }
        
        [field: Tooltip("List of game objects to spawn.")]
        [field: SerializeField] public virtual List<ObjectController> SpawnableObjects { get; set; }
        
        [field: Tooltip("When true will use Spawnable Objects to spawn rather than Spawn Object")]
        [field: SerializeField] public virtual bool SpawnMultiple { get; set; }

        [field: Tooltip("List of available locations for spawning objects.")]
        [field: SerializeField] public virtual List<Vector3> AvailableSpawnLocations { get; set; }
        
        [field: Tooltip("Minimum amount of objects to spawn.")]
        [field: SerializeField] public virtual int AmountToSpawnMin { get; set; }
        
        [field: Tooltip("Maximum amount of objects to spawn.")]
        [field: SerializeField] public virtual int AmountToSpawnMax { get; set; }
        
        [field: Tooltip("Radius for object detection around spawn points.")]
        [field: SerializeField] public virtual Vector3 ObjectDetectRadius { get; set; }
        
        [field: Tooltip("The offset for object spawning")]
        [field: SerializeField] public virtual Vector3 ObjectSpawnOffset { get; set; }
        
        [field: Tooltip("If true, objects are chosen randomly from the ObjectsToSpawn list for spawning.")]
        [field: SerializeField] public virtual bool UseRandomSpawnObjects { get; set; }
        
        [field: Tooltip("When true will spawn objects inside the bounds defined.")]
        [field: SerializeField] public virtual bool UseSpawningBounds { get; set; }
        
        [field: Tooltip("When UseSpawningBounds is true this will show a gizmo in the editor with this color.")]
        [field: SerializeField] public virtual Color BoundsGizmoColor { get; set; } = Color.green;
        
        [field: Tooltip("This the the min bounds for spawning objects")]
        [field: SerializeField] public virtual Vector3 MinBounds { get; set; }
        
        [field: Tooltip("This the the max bounds for spawning objects")]
        [field: SerializeField] public virtual Vector3 MaxBounds { get; set; }
        
        [field: Tooltip("When true it will use the Tilemap found with the Tilemap To Detect Key as the Tilemap to check for available spawn locations")]
        [field: SerializeField] public virtual bool UseTilemapToPopulateAvailableSpawnLocations { get; set; }
        
        [field: Tooltip("ID used to find Tilemap that us used for finding available spawn points.")]
        [field: SerializeField] public virtual SerializableGuid TilemapToDetectID { get; set; }
        
        [field: Tooltip("ID used to find Tilemap that us used as the parent for spawning")]
        [field: SerializeField] public virtual SerializableGuid TilemapToSpawnInsideID { get; set; }
        
        [field: Tooltip("Use object pooling for efficiency.")]
        [field: SerializeField] public virtual bool UseObjectPool { get; set; }
        
        [field: Tooltip("Minimum capacity of the object pool.")]
        [field: SerializeField] public virtual int ObjectPoolMinCapacity { get; set; }
        
        [field: Tooltip("Maximum capacity of the object pool.")]
        [field: SerializeField] public virtual int ObjectPoolMaxCapacity { get; set; }
        
        /// <summary>
        ///  This method handles the spawning trigger message and sends a spawn message.
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Spawning,
                    SpawnMultiple
                        ? new SpawnMessage(SpawnOperation.SpawnAll,
                            spawnGameObjects: SpawnableObjects,
                            spawnPositions: AvailableSpawnLocations,
                            useSpawningBounds: UseSpawningBounds,
                            minBounds: MinBounds, maxBounds: MaxBounds,
                            amountToSpawnMin: AmountToSpawnMin,
                            amountToSpawnMax: AmountToSpawnMax,
                            useRandomSpawnObjects: UseRandomSpawnObjects,
                            objectDetectRadius: ObjectDetectRadius,
                            objectSpawnOffset: ObjectSpawnOffset,
                            useTilemapToPopulateAvailableSpawnLocations: UseTilemapToPopulateAvailableSpawnLocations,
                            tileMapToDetectID: TilemapToDetectID,
                            tilemapToSpawnInsideID: TilemapToSpawnInsideID,
                            useObjectPool: UseObjectPool,
                            objectPoolMinCapacity: ObjectPoolMinCapacity,
                            objectPoolMaxCapacity: ObjectPoolMaxCapacity
                        )
                        : new SpawnMessage(SpawnOperation.SpawnSingle,
                            spawnGameObject: SpawnObject,
                            spawnPositions: AvailableSpawnLocations,
                            useSpawningBounds: UseSpawningBounds,
                            minBounds: MinBounds, maxBounds: MaxBounds,
                            amountToSpawnMin: AmountToSpawnMin,
                            amountToSpawnMax: AmountToSpawnMax,
                            useRandomSpawnObjects: UseRandomSpawnObjects,
                            objectDetectRadius: ObjectDetectRadius,
                            objectSpawnOffset: ObjectSpawnOffset,
                            useTilemapToPopulateAvailableSpawnLocations: UseTilemapToPopulateAvailableSpawnLocations,
                            tileMapToDetectID: TilemapToDetectID,
                            tilemapToSpawnInsideID: TilemapToSpawnInsideID,
                            useObjectPool: UseObjectPool,
                            objectPoolMinCapacity: ObjectPoolMinCapacity,
                            objectPoolMaxCapacity: ObjectPoolMaxCapacity
                        ));
            }
        }
        
        /// <summary>
        ///  This method draws a gizmo in the editor for the spawning bounds.
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