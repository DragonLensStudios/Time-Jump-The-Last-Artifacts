using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Interfaces;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Player;
using PXE.Core.Projectiles;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PXE.Example_Games.Wild_At_Heart.Scripts.Player
{
    public class WAH_PlayerController : PlayerController
    {
        [Header("Game")]
        // forces a private field to get serialized
        [SerializeField]
        // private backing field for the property 
        private Vector3 _respawnPoint;

        // public readonly access
        public Vector3 respawnPoint => _respawnPoint;


        [field: SerializeField] public virtual BaseProjectile ProjectilePrefab { get; set; }
        [field: SerializeField] public virtual ObjectController AfterPivotPrefab { get; set; }
        [field: SerializeField] public virtual Vector3 ProjectileOffset { get; set; } = new(0.25f, 0f, 0f);
        

        // [System.Serializable]
        // public class BalanceSettings
        // {
        //     public float testing;
        // }
 
        [field: Tooltip("default damage reduction Multiplier before modifiers.")]
        [field: SerializeField] public float BaseToughMultiplier { get; set; } = 1.0f;
        [field: Tooltip("fire action cooldown before modifiers.")]
        [field: SerializeField] public float BaseFireCooldown { get; set; } = 1.0f;
        [field: Tooltip("dash action effect distance before modifiers.")]
        [field: SerializeField] public float BaseDashGap { get; set; } = 2.0f;
        [field: Tooltip("dash action effect duration before modifiers.")]
        [field: SerializeField] public float BaseDashDuration { get; set; } = 1.0f;
        [field: Tooltip("dash action cooldown before modifiers.")]
        [field: SerializeField] public float BaseDashCooldown { get; set; } = 1.0f;
        [field: Tooltip("guard action effect duration before modifiers.")]
        [field: SerializeField] public float BaseGuardDuration { get; set; } = 1.0f;
        [field: Tooltip("guard action cooldown before modifiers.")]
        [field: SerializeField] public float BaseGuardCooldown { get; set; } = 1.0f;
        [field: Tooltip("reflect action effect duration before modifiers.")]
        [field: SerializeField] public float BaseSpecialDuration { get; set; } = 1.0f;
        [field: Tooltip("reflect action cooldown before modifiers.")]
        [field: SerializeField] public float BaseSpecialCooldown { get; set; } = 1.0f;
        [field: SerializeField] public ObjectController EffectAnimationObject { get; set; }
        [field: SerializeField] public float AccelMultiplier { get; set; } = 1.0f;
        [field: SerializeField] public float DeaccelMultiplier { get; set; } = 1.0f;
        [field: SerializeField] public ObjectController DashParticle { get; set; }

        // protected virtual float DodgeCountdown { get; set; } = 0f; // in ActorController.cs
        // protected virtual float FireCountdown { get; set; } = 0f; // autofire
        
        protected virtual float DashCountdown { get; set; } = 0f;
        protected virtual float GuardCountdown { get; set; } = 0f;
        protected virtual float ReflectCountdown { get; set; } = 0f;

        protected virtual float FireCooldown { get; set; } = 0f;
        protected virtual float DashCooldown { get; set; } = 0f;
        protected virtual float GuardCooldown { get; set; } = 0f;
        protected virtual float SpecialCooldown { get; set; } = 0f;

        protected virtual float FireQueue { get; set; } = 0f;
        protected virtual float DashQueue { get; set; } = 0f;
        protected virtual float GuardQueue { get; set; } = 0f;
        protected virtual float SpecialQueue { get; set; } = 0f;

        protected virtual Animator EffectAnimationAnimator { get; set; }
    
        protected virtual SpriteRenderer EffectAnimationSpriteRenderer { get; set; }

        protected virtual ParticleSystem[] DashParticleEffects { get; set; }
    
        public override void OnActive()
        {
            base.OnActive();
            MessageSystem.MessageManager.RegisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);

            // downbutton/space = Fire, upbutton = ?, leftbutton = ?, rightbutton = ?, f1 = DebugMenuOpen, select = ?, start/esc = Pause (menu), e = Interact, i = ToggleInventory, q = OpenAchievementMenu

            // WASD/leftjoystick movement, leftclick/leftbutton = shoot (Fire), space/downbutton = dash, shift/upbutton = reflect , rightclick/rightbutton = guard, esc/start = pause, ?/select = ?,
        }

        public override void OnInactive()
        {
            base.OnInactive();
            MessageSystem.MessageManager.UnregisterForChannel<LevelResetMessage>(MessageChannels.Level, LevelResetMessageHandler);

        }

        public override void Start()
        {
            base.Start();
            EffectAnimationAnimator = EffectAnimationObject?.GetComponent<Animator>();
            EffectAnimationSpriteRenderer = EffectAnimationObject?.GetComponent<SpriteRenderer>();
            if (DashParticle != null)
            {
                DashParticleEffects = DashParticle.GetComponentsInChildren<ParticleSystem>();
            }
    
        
        }

        public override void UpdateMovementDirection()
        {
            Vector3 difference = transform.position - LastPosition;
            if (difference != Vector3.zero) MovementDirection = difference;
            if (movement != Vector3.zero) MovementDirection = new Vector3(movement.x, movement.y, movement.z);
            MovementDirection.Normalize();

            if (MovementDirection == Vector3.zero)
            {
                MovementDirection = sr.flipX ? Vector3.left : Vector3.right; // TODO should default to down in the game engine
            }

            float deadzoneDegrees = 45f;
            bool flipX = (MovementDirection.x < -Mathf.Sin(deadzoneDegrees * Mathf.Deg2Rad / 2));
            sr.flipX = flipX;
            if (EffectAnimationObject != null)
            {
                EffectAnimationSpriteRenderer.flipX = flipX;
            }
            if (DashParticle != null)
            {
                DashParticle.transform.localPosition = new Vector3(Mathf.Abs(DashParticle.transform.localPosition.x) * (flipX ? 1f : -1f), DashParticle.transform.localPosition.y, 0f);

                //e.transform.localPosition = new Vector3(-e.transform.localPosition.x, e.transform.localPosition.y, 0f);
                //foreach (var e in DashParticleEffects)
                //{
                //    e.transform.localPosition = new Vector3(Mathf.Abs(e.transform.localPosition.x) * (flipX ? -1f : 1f), e.transform.localPosition.y, 0f);
                //}
            }
        }
    
        public virtual Vector3 GetMouseDirection(float snapAngle = 5f)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 playerPos = transform.position;
            Vector3 mouseDirection = mousePos - playerPos;

            // Normalize to unit direction
            mouseDirection.Normalize();

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;

            // Round the angle to the nearest multiple of the snap angle
            int directionIndex = Mathf.RoundToInt(angle / snapAngle) % (int)(360f / snapAngle);
            float snappedAngle = directionIndex * snapAngle;
            float snappedAngleRadians = snappedAngle * Mathf.Deg2Rad;

            // Convert back to a unit vector
            mouseDirection.x = Mathf.Cos(snappedAngleRadians);
            mouseDirection.y = Mathf.Sin(snappedAngleRadians);

            return mouseDirection;
        }
    
        public void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject == null) return;
            if (other.gameObject == gameObject) return;
            if (IsDisabled) return;
            var target = other.gameObject.GetComponent<IHitable>();
            if (target == null) return;
            if (DashCountdown <= 0f) return;
            HitableProjectile projectile = ProjectilePrefab as HitableProjectile;
            if (projectile != null)
            {
                if (target.OnHit(this, projectile.Damage))
                {
                    Debug.Log("Player rammed target");
                    DashCountdown = 0f;
                }
            }
        }

        public override bool OnHit(IGameObject o, HitType hitType, int damage = 0)
        {
            // base.Hit();
            if (!IsHitable) return false;
            HitableProjectile sourceProjectile = o.gameObject.GetComponent<HitableProjectile>(); // = source as HitableProjectile; 
            if (sourceProjectile != null)
            {
                if (!hitType.HasFlag(HitType.HitOwner) && this == sourceProjectile.Owner) return false;
            }
        
            var sourceHitable = o.gameObject.GetComponent<IHitable>();
            if (sourceHitable != null)
            {
                if (!hitType.HasFlag(HitType.HitTeam) && ((Team & sourceHitable.Team) != 0)) return false;
            }
            if (sourceProjectile != null && ReflectCountdown > 0f && !hitType.HasFlag(HitType.HitDodging))
            {
                sourceProjectile.Owner = this;
                sourceProjectile.LifeCountdown = sourceProjectile.LifeDuration;
                sourceProjectile.Speed *= sourceProjectile.ReflectSpeedMultiplier;
                sourceProjectile.MovementDirection = MovementDirection; // sr.flipX;
                sourceProjectile.Damage = (int)Mathf.Round((float)sourceProjectile.Damage * sourceProjectile.ReflectDamageMultiplier);
                return false;
            }
            if (IsDodgeing && !hitType.HasFlag(HitType.HitDodging)) return false;
            if (IsFlying && hitType.HasFlag(HitType.MissFlying)) return false;
            if (IsInvincible || GodMode) return true;
            //TODO: Status Effects Go Here
            if (MaxHealth <= 0) return true;
            if (damage <= 0) return true;
            if (hitType.HasFlag(HitType.Healing))
            {
                CurrentHealth += damage;
                if (CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }

                return true;
            }
            else
            {
                CurrentHealth -= (int)Mathf.Round((float)damage / BaseToughMultiplier);
                if (CurrentHealth <= 0)
                {
                    OnDie();
                }
                DodgeCountdown = Mathf.Max(DodgeCountdown, DodgeDuration);
                IsDodgeing = true;

                return true;
            }
        }

        public override void MoveOnperformed(InputAction.CallbackContext input)
        {
            base.MoveOnperformed(input);
            EffectAnimationAnimator?.SetBool("isMoving", true);
            if (DashParticle != null)
            {
                foreach (var e in DashParticleEffects)
                {
                    e.Play();
                }
            }
            if (DashCountdown > 0f)
            {
                anim.SetBool("isDashing", true);
                EffectAnimationAnimator?.SetBool("isDashing", true);
            }
            else
            {
                anim.SetBool("isDashing", false);
                EffectAnimationAnimator?.SetBool("isDashing", false);
            }

            UpdateMovementDirection();
        }

        public override void MoveOncanceled(InputAction.CallbackContext input)
        {
            base.MoveOncanceled(input);
            EffectAnimationAnimator?.SetBool("isMoving", false);
            if (DashParticle != null)
            {
                foreach (var e in DashParticleEffects)
                {
                    e.Stop();
                }
            }
        }

        protected float cooldownMultiplier()
        {
            return (4 - (3 * CurrentHealth / MaxHealth)); // 100% to 400% Cooldown
        }

        public override void Update()
        {
            if(IsDisabled) return;
            base.Update();
            if (playerInput == null) return; // shutting down

            //DodgeCountdown = Mathf.Max(DodgeCountdown - UnityEngine.Time.deltaTime, 0f); // in base.Update()
            //FireCountdown = Mathf.Max(FireCountdown - UnityEngine.Time.deltaTime, 0f);
            DashCountdown = Mathf.Max(DashCountdown - UnityEngine.Time.deltaTime, 0f);
            GuardCountdown = Mathf.Max(GuardCountdown - UnityEngine.Time.deltaTime, 0f);
            ReflectCountdown = Mathf.Max(ReflectCountdown - UnityEngine.Time.deltaTime, 0f);

            FireCooldown = Mathf.Max(FireCooldown - UnityEngine.Time.deltaTime, 0f);
            DashCooldown = Mathf.Max(DashCooldown - UnityEngine.Time.deltaTime, 0f);
            GuardCooldown = Mathf.Max(GuardCooldown - UnityEngine.Time.deltaTime, 0f);
            SpecialCooldown = Mathf.Max(SpecialCooldown - UnityEngine.Time.deltaTime, 0f);

            FireQueue = Mathf.Max(FireQueue - UnityEngine.Time.deltaTime, 0f);
            DashQueue = Mathf.Max(DashQueue - UnityEngine.Time.deltaTime, 0f);
            GuardQueue = Mathf.Max(GuardQueue - UnityEngine.Time.deltaTime, 0f);
            SpecialQueue = Mathf.Max(SpecialQueue - UnityEngine.Time.deltaTime, 0f);

            //IsDodgeing = (ReflectCountdown > 0f || DodgeCountdown > 0f) ? true : false ; // in base.Update()
            IsInvincible = (GuardCountdown > 0f || GodMode) ? true : false ;

            if ((playerInput.Player.Fire.IsPressed() || FireQueue > 0f) && FireCooldown <= 0f) {
                var spawnPos = new Vector3(ProjectileOffset.x * (sr.flipX ? -1f : 1f), ProjectileOffset.y);

                if (ProjectilePrefab != null)
                {
                    var projectile = Instantiate(ProjectilePrefab, transform.position + spawnPos, Quaternion.identity);
                    projectile.Owner = this;
                    projectile.MovementDirection = GetMouseDirection(5f);;
                    projectile.Speed = Mathf.Max(projectile.Speed, MoveSpeed + 2.0f);
                }
            
                //FireCountdown = Mathf.Max(FireCountdown, 0.2f); // Autofire
                FireCooldown = Mathf.Max(FireCooldown, BaseFireCooldown * cooldownMultiplier());
            }

            if (DashCountdown <= 0f)
            {
                MoveSpeed = BaseMoveSpeed;
                IsFlying = false;
            }

            //TODO: change control for dash to be what it is expected to be for WAH
            if ((playerInput.Player.Run.IsPressed() || DashQueue > 0f) && DashCooldown <= 0f)
            {
                MoveSpeed = Mathf.Max(BaseMoveSpeed, (BaseDashGap + 2.17f) * 0.71f / BaseDashDuration); // character 2 tiles wide plus 1/6 tile leniency.
                IsFlying = true;

                DashCountdown = Mathf.Max(DashCountdown, BaseDashDuration);
                DashCooldown = Mathf.Max(DashCooldown, BaseDashCooldown * cooldownMultiplier());
            }
            if ((playerInput.Player.Guard.IsPressed() || GuardQueue > 0f) && GuardCooldown <= 0f)
            {
                GuardCountdown = Mathf.Max(GuardCountdown, BaseGuardDuration);
                GuardCooldown = Mathf.Max(GuardCooldown, BaseGuardCooldown * cooldownMultiplier());
            }

            if ((playerInput.Player.Special.IsPressed() || SpecialQueue > 0f) && SpecialCooldown <= 0f)
            {
                ReflectCountdown = Mathf.Max(ReflectCountdown, BaseSpecialDuration);
                SpecialCooldown = Mathf.Max(SpecialCooldown, BaseSpecialCooldown * cooldownMultiplier());
            }

            Color color = sr.color;
            if (CurrentHealth * 3 < MaxHealth)
            {
                float nonredReduction = Mathf.Sin(UnityEngine.Time.unscaledTime * 2f * Mathf.PI) / 4f + (11f / 12f); // .667 to 1.167
                nonredReduction += (float)CurrentHealth / (float)MaxHealth;
                color.g = nonredReduction;
                color.b = nonredReduction;
            }
            else
            {
                color.g = 1f;
                color.b = 1f;
            }
            if (DashCountdown > 0f)
            {
                color.r = 0f;
            }
            else
            {
                color.r = 1f;
            }
            sr.color = color;
        }

        public override void OnDie()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(this, true));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GameOverState>()));
        }

        public override void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            base.LevelResetMessageHandler(message);
            CurrentHealth = MaxHealth;
        }

        // public bool rotate_debug_once = false;

        public override void FixedUpdate()
        {
            if (IsDisabled) return;
        
            Vector2 targetVelocity;
            if (movement.sqrMagnitude > 0.1)
            {
                targetVelocity = Vector2.ClampMagnitude(movement, 1f) * MoveSpeed;
            
                if (DashCountdown <= 0f) {
                    float pivotRotationAngle = -ClockAngle(targetVelocity);
                    if (Vector2.Angle(targetVelocity, rb.linearVelocity) > 90 && rb.linearVelocity.sqrMagnitude > BaseMoveSpeed * BaseMoveSpeed / 25) {

                        // rb.velocity = Vector2.zero; // fallback if pivoted is broken
                    
                        Vector2 currentVelocity = rb.linearVelocity;
                        Vector2 currentVelocityRotated = Rotate(currentVelocity, pivotRotationAngle);
                        var pivotedVelocityRotated = new Vector2(currentVelocityRotated.x, 0f);
                        var pivotedVelocity = Rotate(pivotedVelocityRotated, -pivotRotationAngle);
                        rb.linearVelocity = pivotedVelocity;
                        if (AfterPivotPrefab != null) Instantiate(AfterPivotPrefab, transform.position, transform.rotation);

                        // if (!rotate_debug_once) {
                        //     rotate_debug_once = true;
                        //     // Debug.Log("                      Sin(0*PI): " + Mathf.Sin(0 * Mathf.Deg2Rad));
                        //     // Debug.Log("                     Sin(45*PI): " + Mathf.Sin(45 * Mathf.Deg2Rad));
                        //     // Debug.Log("                     Sin(90*PI): " + Mathf.Sin(90 * Mathf.Deg2Rad));
                        //     // Debug.Log("                    Sin(135*PI): " + Mathf.Sin(135 * Mathf.Deg2Rad));
                        //     // Debug.Log("                    Sin(180*PI): " + Mathf.Sin(180 * Mathf.Deg2Rad));
                        //     // Debug.Log("                    Sin(270*PI): " + Mathf.Sin(270 * Mathf.Deg2Rad));
                        //     // Debug.Log("                    Sin(360*PI): " + Mathf.Sin(360 * Mathf.Deg2Rad));
                        //     Debug.Log("                             up: " + Vector2.up);
                        //     Debug.Log("                             up: " + Rotate(Vector2.up, 0));
                        //     Debug.Log("                          right: " + Vector2.right);
                        //     Debug.Log("                          right: " + Rotate(Vector2.up, 90));
                        //     Debug.Log("                           down: " + Vector2.down);
                        //     Debug.Log("                           down: " + Rotate(Vector2.up, 180));
                        //     Debug.Log("                           left: " + Vector2.left);
                        //     Debug.Log("                           left: " + Rotate(Vector2.up, -90));
                        //     Debug.Log("                          value: " + currentVelocity);
                        //     Debug.Log("                         target: " + targetVelocity);
                        //     Debug.Log("         angle(value_rotate_up): " + ClockAngle(currentVelocity));
                        //     Debug.Log("        angle(target_rotate_up): " + ClockAngle(targetVelocity));
                        //     Debug.Log("fix_a= -angle(target_rotate_up): " + pivotRotationAngle);
                        
                        //     Debug.Log("               value_rotate_fix: " + currentVelocityRotated);
                        //     Debug.Log("        angle(value_rotate_fix): " + ClockAngle(currentVelocityRotated));
                        
                        //     Debug.Log("             pivoted_rotate_fix: " + pivotedVelocityRotated);
                        //     Debug.Log("      angle(pivoted_rotate_fix): " + ClockAngle(pivotedVelocityRotated));

                        //     Debug.Log("                        pivoted: " + pivotedVelocity);
                        //     Debug.Log("                 angle(pivoted): " + ClockAngle(pivotedVelocity));
                        // }
                    }
                    rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, AccelMultiplier * MoveSpeed * 3f / 50f);
                }
                else
                {
                    rb.linearVelocity = targetVelocity;
                }
            } else {
                targetVelocity = Vector2.zero;
                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, DeaccelMultiplier * BaseMoveSpeed * 3f / 50f);
            }
        }

        public static float ClockAngle(Vector2 vector)
        {
            return Vector2.SignedAngle(vector, Vector2.up);
        }

        public static Vector2 Rotate(Vector2 v, float angle)
        {
            return new Vector2(v.x * Mathf.Cos(angle * -Mathf.Deg2Rad) - v.y * Mathf.Sin(angle * -Mathf.Deg2Rad), v.x * Mathf.Sin(angle * -Mathf.Deg2Rad) + v.y * Mathf.Cos(angle * -Mathf.Deg2Rad));
        }
    }
}
