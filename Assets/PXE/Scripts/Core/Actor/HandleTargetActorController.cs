using System;
using System.Linq;
using PXE.Core.Enums;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.Actor
{
    /// <summary>
    ///  Represents the ChaseTargetObjectController.
    /// </summary>
    public class HandleTargetActorController : ActorController
    {
        [field: Tooltip("The target.")]
        [field: SerializeField] public virtual Transform Target { get; set; }
        
        [field: SerializeField] public virtual float FindTargetFocusDuration { get; set; } = 0.25f;
        
        [field: Tooltip("The detection radius.")]
        [field: SerializeField] public virtual float DetectionRadius { get; set; } = 5.0f;
        
        [field: Tooltip("The stop move radius.")]
        [field: SerializeField] public virtual float StopMoveRadius { get; set; } = 1.0f;
        
        [field: Tooltip("The chase speed.")]
        [field: SerializeField] public virtual float ChaseSpeed { get; set; } = 3.0f;
        
        [field: Tooltip("The lost sight range.")]
        [field: SerializeField] public virtual float LostSightRange { get; set; } = 7.0f;
        
        [field: Tooltip("Chase the target.")]
        [field: SerializeField] public virtual bool ChaseTarget { get; set; } = true;
        
        [field: Tooltip("The prefab to instantiate after switching to a new target.")]
        [field: SerializeField] public virtual ObjectController AfterTargetPrefab { get; set; }

        
        protected bool isChasing = false;
        protected float findTargetFocusCountdown = 0.0f;

        /// <summary>
        ///  Checks if the enemy has line of sight to the player or the current waypoint.
        /// </summary>
        /// <param name="waypoint"></param>
        /// <returns></returns>
        public override bool HasLineOfSight(Vector2 waypoint)
        {
            if (CanSeeTarget()) return true;
            if (PatrolPoints.Count == 0) return true;
            Vector2 currentWaypoint = PatrolPoints[CurrentPatrolIndex];
            return base.HasLineOfSight(currentWaypoint);
        }

        /// <summary>
        ///  Checks if the enemy can see the player.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSeeTarget()
        {
            if (Target == null) return false;
            var position = transform.position;
            Vector2 directionToTarget = (Target.position - position).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(position, directionToTarget, DetectionRadius, ObstacleLayerMask);

            foreach (RaycastHit2D hit in hits)
            {
                // Ignore the hit if it's itself
                if (hit.transform == transform)
                    continue;

                var hitActor = hit.transform.GetComponent<ActorController>();
                if (hitActor != null)
                {
                    if ((Team & hitActor.Team) != 0) continue;
                    
                    return true;
                }

                // If you hit something else before the player, the line of sight is blocked
                if (hit.collider != null) 
                    continue;
            }

            return false;
        }
        
        public virtual Transform FindTarget()
        {
            Transform target = null;
            var position = transform.position;
            var colliders = Physics2D.OverlapCircleAll(position, DetectionRadius, ObstacleLayerMask);
            var filteredColliders = colliders.Where(x =>
            { 
                var actor = x.GetComponent<ActorController>();
                if (actor == null) return false;
                if (actor.Team == TeamType.Neutrals) return false;
                if ((Team & actor.Team) != 0) return false;
                return true;
            });
            
            // Sets the target to the closest actor.
            var newTarget = filteredColliders.OrderBy(x=> Vector2.Distance(x.transform.position, position)).FirstOrDefault()?.transform;
            
            if (newTarget != null && Target != newTarget )
            {
                // Debug.Log($"Target: {target?.gameObject.name} found.");
                if (AfterTargetPrefab != null) Instantiate(AfterTargetPrefab, position, Quaternion.identity);
            }
            target = newTarget;
            return target;
        }

        /// <summary>
        ///  If the enemy is chasing the player, it will move towards the player.
        /// </summary>
        public override void FixedUpdate()
        {
            if (IsDisabled) return;
            if (!ChaseTarget)
            {
                Patrol();
                return;
            }
            findTargetFocusCountdown = Mathf.Max(findTargetFocusCountdown - UnityEngine.Time.deltaTime, 0f);

            if (findTargetFocusCountdown <= 0f)
            {
                findTargetFocusCountdown = FindTargetFocusDuration;
                Target = FindTarget();
            }
            
            if (Target == null)
            {
                Patrol();
                return;
            }
            float distanceToTarget = Vector2.Distance(rb.position, Target.position);

            //TODO: Fix this so that when out of sight range, the enemy will stop chasing and reset the target.
            // Check if the target is null or out of sight range
            if (Target == null || distanceToTarget > LostSightRange) 
            {
                // Stop chasing and reset the target if it's null or out of range.
                isChasing = false;
                Target = null;
                return;
            }
            else 
            {
                // Update isChasing ONLY when the target is not null and within sight range
                isChasing = CanSeeTarget() && distanceToTarget <= DetectionRadius;
            }
            
            if (isChasing)
            {
                if (Target == null) return;
                var position = Target.position;
                Vector2 directionToTarget = ((Vector2)position - rb.position).normalized;
                //TODO:Add handling to jump obstacles and traverse to reach the waypoints
                switch (GameViewType)
                {
                    case GameViewType.TopDown:
                        rb.linearVelocity = new Vector2(directionToTarget.x * ChaseSpeed, directionToTarget.y * ChaseSpeed);
                        Debug.Log("Topdown Chasing: " + rb.linearVelocity);
                        break;
                    case GameViewType.SideView:
                        rb.linearVelocity = new Vector2(directionToTarget.x * ChaseSpeed, rb.linearVelocity.y);
                        Debug.Log("Sideview Chasing: " + rb.linearVelocity);
                        break;
                    default:
                        //TODO: Add handling for mor GameViewTypes for Chasing.
                        Debug.LogWarning("Chasing behavior for " + GameViewType + " is not implemented.");
                        break;
                }
            }
            else
            {
                Patrol();
            }
        }
        
        /// <summary>
        ///  Draws the detection radius and lost sight range.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, DetectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, LostSightRange);

            if (Target == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + (Target.position - position).normalized * DetectionRadius);

        }
    }
}