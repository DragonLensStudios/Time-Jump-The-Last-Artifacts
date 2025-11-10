using System.Collections.Generic;
using PXE.Core.Data_Persistence.Data;
using PXE.Core.Time;
using UnityEngine;

namespace PXE.Example_Games.Oceans_Call.Data_Persistence.Data
{
    [System.Serializable]
    public class OceansCallGameData : BaseGameData
    {
        [field: SerializeField] public virtual int CurrentLives { get; set; }
        [field: SerializeField] public virtual List<Vector3> ReachedPositions { get; set; }
        [field: SerializeField] public virtual float LastDepthTriggered { get; set; }
        [field: SerializeField] public virtual List<float> DialogueTriggerDepths { get; set; }
        [field: SerializeField] public virtual Vector3 CurrentWayPoint { get; set; }
        [field: SerializeField] public virtual bool ReachedLastWaypoint { get; set; }
        
        public OceansCallGameData()
        {
            CurrentLives = 3;
            ReachedPositions = new List<Vector3>();
            LastDepthTriggered = 0;
            DialogueTriggerDepths = new List<float>();
            CurrentWayPoint = new Vector3(0, 0);
            ReachedLastWaypoint = false;
        }

        public OceansCallGameData(int currentLives, GameTimeObject playerCurrentTime, string diverReferenceState, Vector3 diverPosition, List<Vector3> reachedPositions, float diverSpeed, float lastDepthTriggered, List<float> dialogueTriggerDepths, Vector3 currentWayPoint, bool reachedLastWaypoint)
        {
            CurrentLives = currentLives;
            ReachedPositions = reachedPositions;
            LastDepthTriggered = lastDepthTriggered;
            DialogueTriggerDepths = dialogueTriggerDepths;
            CurrentWayPoint = currentWayPoint;
            ReachedLastWaypoint = reachedLastWaypoint;
        }
    }
}