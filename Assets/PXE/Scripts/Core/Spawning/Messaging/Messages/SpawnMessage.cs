#nullable enable
using System.Collections.Generic;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Objects;
using PXE.Core.SerializableTypes;
using UnityEngine;

namespace PXE.Core.Spawning.Messaging.Messages
{
    public struct SpawnMessage
    {
        public ObjectController? ObjectToSpawn { get; }
        public List<ObjectController>? SpawnGameObjects { get; }
        public SpawnOperation SpawnOperation { get; }
        public List<Vector3>? SpawnPositions { get; }
        public bool? UseSpawningBounds { get; }
        public Vector3? MinBounds { get; }
        public Vector3? MaxBounds { get; }
        public bool? UseRandomSpawnObjects { get; }
        public int? AmountToSpawnMin { get; }
        public int? AmountToSpawnMax { get; }
        public Vector3? ObjectDetectRadius { get; }
        public Vector3? ObjectSpawnOffset { get; }
        public bool? UseTilemapToPopulateAvailableSpawnLocations { get; }
        public SerializableGuid? TileMapToDetectID { get; }
        public SerializableGuid? TilemapToSpawnInsideID { get; }
        public bool? UseObjectPool { get; }
        public int? ObjectPoolMinCapacity { get; }
        public int? ObjectPoolMaxCapacity { get; }

/// <summary>
/// Executes the SpawnMessage method.
/// Handles the SpawnMessage functionality.
/// </summary>
        public SpawnMessage(SpawnOperation spawnOperation, List<ObjectController>? spawnGameObjects = null, List<Vector3>? spawnPositions = null, bool? useSpawningBounds = null, Vector3? minBounds = null, Vector3? maxBounds = null, int? amountToSpawnMin = null, int? amountToSpawnMax = null, bool? useRandomSpawnObjects = null, Vector3? objectDetectRadius = null, Vector3? objectSpawnOffset = null, bool? useTilemapToPopulateAvailableSpawnLocations = null, SerializableGuid? tileMapToDetectID = null, SerializableGuid? tilemapToSpawnInsideID = null, bool? useObjectPool = null, int? objectPoolMinCapacity = null, int? objectPoolMaxCapacity = null )
        {
            SpawnOperation = spawnOperation;
            ObjectToSpawn = spawnGameObjects?.FirstOrDefault();
            SpawnGameObjects = spawnGameObjects;
            SpawnPositions = spawnPositions;
            UseSpawningBounds = useSpawningBounds;
            MinBounds = minBounds;
            MaxBounds = maxBounds;
            AmountToSpawnMin = amountToSpawnMin;
            AmountToSpawnMax = amountToSpawnMax;
            ObjectDetectRadius = objectDetectRadius;
            ObjectSpawnOffset = objectSpawnOffset;
            UseTilemapToPopulateAvailableSpawnLocations = useTilemapToPopulateAvailableSpawnLocations;
            UseRandomSpawnObjects = useRandomSpawnObjects;
            TileMapToDetectID = tileMapToDetectID;
            TilemapToSpawnInsideID = tilemapToSpawnInsideID;
            UseObjectPool = useObjectPool;
            ObjectPoolMinCapacity = objectPoolMinCapacity;
            ObjectPoolMaxCapacity = objectPoolMaxCapacity;
        }
        
/// <summary>
/// Executes the SpawnMessage method.
/// Handles the SpawnMessage functionality.
/// </summary>
        public SpawnMessage(SpawnOperation spawnOperation, ObjectController? spawnGameObject = null,  List<Vector3>? spawnPositions = null, bool? useSpawningBounds = null, Vector3? minBounds = null, Vector3? maxBounds = null, int? amountToSpawnMin = null, int? amountToSpawnMax = null, bool? useRandomSpawnObjects = null, Vector3? objectDetectRadius = null, Vector3? objectSpawnOffset = null, bool? useTilemapToPopulateAvailableSpawnLocations = null, SerializableGuid? tileMapToDetectID = null, SerializableGuid? tilemapToSpawnInsideID = null, bool? useObjectPool = null, int? objectPoolMinCapacity = null, int? objectPoolMaxCapacity = null)
        {
            SpawnOperation = spawnOperation;
            ObjectToSpawn = spawnGameObject;
            SpawnGameObjects = spawnGameObject != null ? new List<ObjectController> { spawnGameObject } : new List<ObjectController>();
            SpawnPositions = spawnPositions;
            UseSpawningBounds = useSpawningBounds;
            MinBounds = minBounds;
            MaxBounds = maxBounds;
            AmountToSpawnMin = amountToSpawnMin;
            AmountToSpawnMax = amountToSpawnMax;
            ObjectDetectRadius = objectDetectRadius;
            ObjectSpawnOffset = objectSpawnOffset;
            UseTilemapToPopulateAvailableSpawnLocations = useTilemapToPopulateAvailableSpawnLocations;
            UseRandomSpawnObjects = useRandomSpawnObjects;
            TileMapToDetectID = tileMapToDetectID;
            TilemapToSpawnInsideID = tilemapToSpawnInsideID;
            UseObjectPool = useObjectPool;
            ObjectPoolMinCapacity = objectPoolMinCapacity;
            ObjectPoolMaxCapacity = objectPoolMaxCapacity;
        }
        
    }
}