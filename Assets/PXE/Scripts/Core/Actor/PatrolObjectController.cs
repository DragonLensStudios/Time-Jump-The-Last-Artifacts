using System;
using System.Collections;
using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Objects;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;

namespace PXE.Core.Actor
{
    /// <summary>
    ///  Represents the PatrolObjectController.
    /// </summary>
    public class PatrolObjectController : ObjectController
    {
        [field: Tooltip("The Game View Type (Perspective)")]
        [field: SerializeField] public virtual GameViewType GameViewType { get; set; } = GameViewType.TopDown;
        
        [field: Tooltip("The default move speed before modifiers.")]
        [field: SerializeField] public virtual float BaseMoveSpeed { get; set; } = 3f;
        
        [field: Tooltip("The current move speed of the actor.")]
        [field: SerializeField] public virtual float MoveSpeed { get; set; }
        
        [field: Tooltip("The patrol points.")]
        [field: SerializeField] public virtual List<Vector2> PatrolPoints { get; set; }
        
        [field: Tooltip("The patrol speed.")]
        [field: SerializeField] public virtual float PatrolSpeed { get; set; } = 2.0f;
        
        [field: Tooltip("The waypoint reached threshold.")]
        [field: SerializeField] public virtual float WaypointReachedThreshold { get; set; } = 0.1f;
        
        [field: Tooltip("The time to reach waypoint.")]
        [field: SerializeField] public virtual float TimeToReachWaypoint { get; set; } = 30.0f;
        
        [field: Tooltip("The wait time at waypoint.")]
        [field: SerializeField] public virtual float WaitTimeAtWaypoint { get; set; } = 2.0f; // Time to wait at each waypoint
        
        [field: Tooltip("Is movement disabled?")]
        [field: SerializeField] public virtual bool IsDisabled { get; set; }
        
        [field: Tooltip("Use patrol?")]
        [field: SerializeField] public virtual bool UsePatrol { get; set; } = false;
        
        [field: Tooltip("The current patrol mode.")]
        [field: SerializeField] public virtual PatrolMode CurrentPatrolMode { get; set; } = PatrolMode.None;
        
        [field: Tooltip("The obstacle layer mask.")]
        [field: SerializeField] public virtual LayerMask ObstacleLayerMask { get; set; }
        
        [field: Tooltip("Ignore object controllers for line of sight?")]
        [field: SerializeField] public virtual bool IgnoreObjectControllersForLineOfSight { get; set; }
        
        [field: Tooltip("Is the object reversing?")]
        [field: SerializeField] public virtual bool IsReversing  { get; set; }
        
        [field: Tooltip("Is the object waiting?")]
        [field: SerializeField] public virtual bool IsWaiting  { get; set; }
        
        [field: Tooltip("The current patrol index.")]
        [field: SerializeField] public virtual int CurrentPatrolIndex  { get; set; }
        
        [field: Tooltip("The time since last waypoint.")]
        [field: SerializeField] public virtual float TimeSinceLastWaypoint  { get; set; }
        
        [field: Tooltip("The reached points.")]
        [field: SerializeField] public virtual List<Vector3> ReachedPoints  { get; set; }
        
        [field: Tooltip("The current waypoint.")]
        [field: SerializeField] public virtual Vector2 CurrentWaypoint  { get; set; }
        
        [field: Tooltip("Has reached last waypoint?")]
        [field: SerializeField] public virtual bool ReachedLastWaypoint { get; set; }
        
        [field: Tooltip("The starting position of the object")]
        [field: SerializeField] public virtual Vector3 StartingPosition { get; set; }
        
        [field: Tooltip("The original gravity scale of the object")]
        [field: SerializeField] public virtual float OriginalGravityScale { get; set; }

        protected Rigidbody2D rb;
        protected SpriteRenderer sr;
        protected Animator anim;


        /// <summary>
        ///  Sets the starting position and original gravity scale.
        /// </summary>
        public override void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            StartingPosition = transform.position;
            OriginalGravityScale = rb.gravityScale;
            base.Start();
        }

        /// <summary>
        ///  This method registers the PatrolObjectController for the PauseMessage message and the LevelResetMessage message.
        /// </summary>
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
        }

        /// <summary>
        ///  This method unregisters the PatrolObjectController for the PauseMessage message and the LevelResetMessage message.
        /// </summary>
        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);
        }
        
        /// <summary>
        ///  This method handles the patrolling and checks the loop, one way, ping pong, and dynamic patrol modes and tries to move to the next waypoint.
        /// </summary>
        public virtual void Patrol()
        {
            if (!ShouldPatrol()) return;

            Debug.Log("Patrolling");
            switch (GameViewType)
            {
                case GameViewType.TopDown:
                    PatrolTopDown();
                    break;
                case GameViewType.SideView:
                    PatrolSideView();
                    break;
                default:
                    //TODO: Add patrol behaviors for other GameViewTypes
                    Debug.LogWarning("Patrol behavior for " + GameViewType + " is not implemented.");
                    break;
            }
        }
        
        public virtual void PatrolTopDown()
        {
            HandlePatrolMovement();  // Uses existing logic suitable for TopDown
        }

        public virtual void PatrolSideView()
        {
            // Adjust the waypoint for horizontal movement only
            CurrentWaypoint = new Vector2(PatrolPoints[CurrentPatrolIndex].x, rb.position.y);
            
            if (HasReached(CurrentWaypoint) || TimeSinceLastWaypoint > TimeToReachWaypoint || !HasLineOfSight(CurrentWaypoint))
            {
                ProcessWaypointReached();
                StartCoroutine(WaitAtWaypoint());
                return;
            }

            MoveHorizontallyTowardsWaypoint();
        }
        
        public virtual void MoveHorizontallyTowardsWaypoint()
        {
            anim.SetBool("isMoving", true);
            sr.flipX = CurrentWaypoint.x < rb.position.x;
            Vector2 horizontalDirection = (new Vector2(CurrentWaypoint.x, rb.position.y) - rb.position).normalized;
            rb.linearVelocity = new Vector2(horizontalDirection.x * PatrolSpeed, rb.position.y);
            TimeSinceLastWaypoint += UnityEngine.Time.fixedDeltaTime;
        }
        
        public virtual void HandlePatrolMovement()
        {
            CurrentWaypoint = PatrolPoints[CurrentPatrolIndex];

            if (HasReached(CurrentWaypoint) || TimeSinceLastWaypoint > TimeToReachWaypoint || !HasLineOfSight(CurrentWaypoint))
            {
                ProcessWaypointReached();
                StartCoroutine(WaitAtWaypoint());
                return;
            }

            MoveTowardsWaypoint();
        }
        
        public virtual void ProcessWaypointReached()
        {
            ReachedPoints.Add(CurrentWaypoint);
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isMoving", false);
            var finalPosition = CurrentPatrolIndex + 1 >= PatrolPoints.Count;
            if (finalPosition)
            {
                ReachedPoints.Clear();
            }
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Gameplay, new PatrolPointReachedMessage(ID, CurrentWaypoint, ReachedPoints, finalPosition, false));
            TimeSinceLastWaypoint = 0.0f;
        }
        
        public virtual void MoveTowardsWaypoint()
        {
            anim.SetBool("isMoving", true);
            Vector2 directionToWaypoint = (CurrentWaypoint - rb.position).normalized;
            rb.linearVelocity = new Vector2(directionToWaypoint.x * PatrolSpeed, directionToWaypoint.y * PatrolSpeed);
            TimeSinceLastWaypoint += UnityEngine.Time.fixedDeltaTime;
        }
        
        public virtual bool ShouldPatrol()
        {
            var isLooping = CurrentPatrolMode is PatrolMode.Loop or PatrolMode.PingPong or PatrolMode.Dynamic;
            return PatrolPoints.Count > 0 && !IsWaiting && UsePatrol && !IsDisabled && (!ReachedLastWaypoint || isLooping);
        }


        /// <summary>
        ///  This method waits at the waypoint for the specified amount of time.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator WaitAtWaypoint()
        {
            IsWaiting = true;
            yield return new WaitForSeconds(WaitTimeAtWaypoint);
            MoveToNextWaypoint();
            IsWaiting = false;
        }

        /// <summary>
        ///  This method sets the closest patrol index.
        /// </summary>
        public virtual void SetClosestPatrolIndex()
        {
            float closestDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < PatrolPoints.Count; i++)
            {
                float currentDistance = Vector2.Distance(rb.position, PatrolPoints[i]);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestIndex = i;
                }
            }

            CurrentPatrolIndex = closestIndex;
            CurrentWaypoint = PatrolPoints[closestIndex];
        }

        /// <summary>
        ///  This method moves to the next waypoint based on the current patrol mode.
        /// </summary>
        public virtual void MoveToNextWaypoint()
        {
            switch (CurrentPatrolMode)
            {
                case PatrolMode.Loop:
                    CurrentPatrolIndex = (CurrentPatrolIndex + 1) % PatrolPoints.Count;
                    break;
        
                case PatrolMode.OneWay:
                    if (CurrentPatrolIndex + 1 < PatrolPoints.Count)
                    {
                        CurrentPatrolIndex++;
                    }
                    else
                    {
                        ReachedLastWaypoint = true;
                    }
                    break;
                
                case PatrolMode.OneWayLoop:
                    if (CurrentPatrolIndex + 1 < PatrolPoints.Count)
                    {
                        CurrentPatrolIndex++;
                    }
                    else
                    {
                        CurrentPatrolIndex = 0;
                        transform.position = PatrolPoints[0];
                        rb.linearVelocity = Vector2.zero;
                        ReachedPoints.Clear();
                    }
                    break;
        
                case PatrolMode.PingPong:
                    if (IsReversing)
                    {
                        if (CurrentPatrolIndex == 0)
                            IsReversing = false;
                        else
                            CurrentPatrolIndex--;
                    }
                    else
                    {
                        if (CurrentPatrolIndex + 1 == PatrolPoints.Count)
                        {
                            IsReversing = true;
                            CurrentPatrolIndex--;
                        }
                        else
                            CurrentPatrolIndex++;
                    }
                    break;
        
                case PatrolMode.Dynamic:
                    // Added the following line to ensure the nearest waypoint is chosen in dynamic mode:
                    SetClosestPatrolIndex();
                    break;
            }
        }

        /// <summary>
        ///  This method checks if the object has line of sight to the waypoint.
        /// </summary>
        /// <param name="waypoint"></param>
        /// <returns></returns>
        public virtual bool HasLineOfSight(Vector2 waypoint)
        {
            switch (GameViewType)
            {
                case GameViewType.TopDown:
                    return CheckLineOfSightTopDown(waypoint);
                case GameViewType.SideView:
                    return CheckLineOfSightSideView(waypoint);
                default:
                    //TODO: Add Line of sight checks for other GameViewTypes
                    Debug.LogWarning("Line of sight check for " + GameViewType + " is not implemented.");
                    break;
            }

            return false;
        }
        
        public virtual bool CheckLineOfSightTopDown(Vector2 waypoint)
        {
            // Calculate the direction to the waypoint from the current position
            Vector2 direction = (waypoint - rb.position).normalized;
            float distance = Vector2.Distance(rb.position, waypoint);

            // Perform a raycast to detect obstacles in the line of sight
            RaycastHit2D[] hits = Physics2D.RaycastAll(rb.position, direction, distance, ObstacleLayerMask);

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    // Retrieve the ObjectController component, if it exists
                    var oc = hit.collider.GetComponent<ObjectController>();

                    // Log a warning if the hit object does not have an ObjectController component
                    if (oc == null)
                    {
                        Debug.LogWarning($"{hit.collider.name} does not have an ObjectController component and will be considered as blocking line of sight.");
                        return false; // Assume objects without ObjectController block line of sight unless specified otherwise
                    }

                    // Skip the check if ignoring ObjectControllers and this object has one
                    if (IgnoreObjectControllersForLineOfSight && oc != null)
                        continue;

                    // If a valid ObjectController is found and it's not the current object, line of sight is blocked
                    if (!oc.ID.Equals(ID))
                        return false;
                }
            }

            return true; // If no obstructions are found, line of sight is clear
        }

        
        public virtual bool CheckLineOfSightSideView(Vector2 waypoint)
        {
            // Calculate the horizontal direction to the waypoint
            Vector2 horizontalDirection = (new Vector2(waypoint.x, rb.position.y) - rb.position).normalized;
            float horizontalDistance = Mathf.Abs(waypoint.x - rb.position.x);

            // Perform a raycast in the horizontal direction
            RaycastHit2D[] hits = Physics2D.RaycastAll(rb.position, horizontalDirection, horizontalDistance, ObstacleLayerMask);
    
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    // Retrieve the ObjectController component, if it exists
                    var oc = hit.collider.GetComponent<ObjectController>();
            
                    // Log a warning if the hit object does not have an ObjectController component
                    if (oc == null)
                    {
                        Debug.LogWarning($"{hit.collider.name} does not have an ObjectController component.");
                        continue; // Skip to the next hit if ignoring objects without the component
                    }

                    // Skip the check if ignoring ObjectControllers and this object has one
                    if (IgnoreObjectControllersForLineOfSight && oc != null)
                        continue;

                    // If a valid ObjectController is found and it's not the current object, line of sight is blocked
                    if (!oc.ID.Equals(ID))
                        return false;
                }
            }

            return true; // If no obstructions are found, line of sight is clear
        }

        
        /// <summary>
        ///  This method checks if the object has reached the waypoint.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool HasReached(Vector2 point)
        {
            float distanceToWaypoint = Vector2.Distance(rb.position, point);
            return distanceToWaypoint < WaypointReachedThreshold;
        }
        
        /// <summary>
        ///  This method handles the PauseMessage message and sets the movement disabled state.
        /// </summary>
        /// <param name="message"></param>
        public virtual void PauseMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            //TODO: Pause on objects is not working correctly. Need to fix this. Need to add a delay so that objects are loaded before any of the pause menu is shown.
            if (!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().GetValueOrDefault();
            if (data.IsPaused)
            {
                if (rb != null)
                {
                    rb.gravityScale = 0;
                    rb.linearVelocity = Vector2.zero;
                }
                if (anim != null)
                {
                    anim.enabled = false;
                }
                IsDisabled = true;
            }
            else
            {
                if (rb != null)
                {
                    rb.gravityScale = OriginalGravityScale;
                }
                if (anim != null)
                {
                    anim.enabled = true;
                }
                IsDisabled = false;
            }
            
        }
        
        /// <summary>
        ///  This method handles the FixedUpdate event and calls the Patrol method.
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsDisabled) return;
            Patrol();
        }
        
        /// <summary>
        ///  This method handles the LevelResetMessage message and resets the object to the starting position.
        /// </summary>
        /// <param name="message"></param>
        public virtual void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<LevelResetMessage>().HasValue) return;
            var data = message.Message<LevelResetMessage>().GetValueOrDefault();
            
            if (transform != null)
            {
                transform.position = StartingPosition;
            }

            // Reset the properties related to patrol state
            CurrentPatrolIndex = 0;
            ReachedLastWaypoint = false;
            IsReversing = false;
            IsWaiting = false;
            TimeSinceLastWaypoint = 0;
            ReachedPoints = new List<Vector3>();
    
            if (PatrolPoints.Count > 0)
            {
                CurrentWaypoint = PatrolPoints[0];
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnInactive();
        }
    }
}