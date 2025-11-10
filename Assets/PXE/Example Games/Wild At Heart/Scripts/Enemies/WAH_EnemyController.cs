using System.Collections;
using PXE.Core.Audio.Messaging.Messages;
using PXE.Core.Enemy;
using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Messaging;
using PXE.Core.Objects;
using PXE.Core.Projectiles;
using UnityEngine;

namespace PXE.Example_Games.Wild_At_Heart.Scripts.Enemies
{
    public class WAH_EnemyController : EnemyActorController
    {
        [field: SerializeField] public virtual CombatType AllowedCombatTypes { get; set; } = CombatType.Melee;
        [field: SerializeField] public virtual float MeleeRange { get; set; } = 1.0f;
        [field: SerializeField] public virtual float RangedRange { get; set; } = 5.0f;
        [field: SerializeField] public virtual BaseProjectile RangedProjectilePrefab { get; set; }
        [field: SerializeField] public virtual ObjectController AfterMeleeAttackPrefab {get; set;}
        [field: SerializeField] public virtual ObjectController AfterRangedAttackPrefab {get; set;}
        [field: SerializeField] public virtual ObjectController AfterHitPrefab { get; set; }
        [field: SerializeField] public virtual float RespawnTime { get; set; } = 30.0f;
    
        protected Coroutine attackCoroutine;
        [SerializeField] protected CombatType currentCombatType = CombatType.None;

        protected Collider2D[] colliders;


        public override void Start()
        {
            base.Start();
            colliders = GetComponents<Collider2D>();
        }

        public override void OnCollisionStay2D(Collision2D other) {}

        public override void Update()
        {
            base.Update();
            if (IsDisabled)
            {
                if(attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
                return;
            }
            if (Target != null)
            {
                if(AllowedCombatTypes.HasFlag(CombatType.Melee) && Vector2.Distance(transform.position, Target.position) <= MeleeRange)
                {
                    currentCombatType = CombatType.Melee;
                }
                else if (AllowedCombatTypes.HasFlag(CombatType.Ranged) && Vector2.Distance(transform.position, Target.position) <= RangedRange)
                {
                    currentCombatType = CombatType.Ranged;
                }
                else if (Vector2.Distance(transform.position, Target.position) > RangedRange)
                {
                    currentCombatType = CombatType.None;
                }
                if (isAttacking) return;
                if (currentCombatType == CombatType.None) return;
                if (attackCoroutine != null) return;
                attackCoroutine = StartCoroutine(AttackTarget(Target.gameObject, currentCombatType));

            }
        }

        public IEnumerator AttackTarget(GameObject target, CombatType combatType = CombatType.Melee)
        {
            isAttacking = true;
            anim?.SetTrigger("Attack");
        
            if (currentCombatType == CombatType.Melee)
            {
                var targetHit = target.gameObject.GetComponent<IHitable>();
                if (targetHit == null) yield break;

                if (Vector2.Distance(transform.position, Target.position) <= MeleeRange)
                {
                    if (AfterMeleeAttackPrefab != null)
                    { 
                        var meleeAttackEffect = Instantiate(AfterMeleeAttackPrefab, transform.position, Quaternion.identity);
                    }
                    if (targetHit.OnHit(this, Damage))
                    {
                        Debug.Log($"{gameObject.name} hit {target.name} for {Damage}");
                    }
                }
            
                if (MeleeDamageSfx != null)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(MeleeDamageSfx, AudioOperation.Play, AudioChannel.SoundEffects));
                    yield return new WaitForSeconds(TimeDelayAfterSfx);
                }
            
            }
            else if (currentCombatType == CombatType.Ranged)
            {
                if (Vector2.Distance(transform.position, Target.position) <= RangedRange)
                {
                    if (AfterRangedAttackPrefab != null)
                    { 
                        var rangeAttackEffect = Instantiate(AfterMeleeAttackPrefab, transform.position, Quaternion.identity);
                    }
                    if (RangedProjectilePrefab != null)
                    {
                        var projectile = Instantiate(RangedProjectilePrefab, transform.position, Quaternion.identity);
                        projectile.Owner = this;
                        projectile.MovementDirection = (Target.position - transform.position).normalized;
                        projectile.Speed = Mathf.Max(projectile.Speed, MoveSpeed + 2.0f);
                    }
                }
            }

            yield return new WaitForSeconds(HitDelay);
            isAttacking = false;
            attackCoroutine = null;
        }

        public override bool OnHit(IGameObject o, HitType hitType, int damage = 0)
        {
            var hit = base.OnHit(o, hitType, damage);
            // Debug.Log($"{gameObject.name} was {hit ? "hit" : "missed"} by {source.name} with {damage}");
            if (hit)
            {
                anim?.SetTrigger("Hit");
                if (AfterHitPrefab != null)
                {
                    var projectile = Instantiate(RangedProjectilePrefab, transform.position, Quaternion.identity);
                    projectile.Owner = this;
                    projectile.MovementDirection = (Target.position - transform.position).normalized;
                    projectile.Speed = Mathf.Max(projectile.Speed, MoveSpeed + 2.0f);
                }
            }
            return hit;
        }

        public override void OnDie()
        {
            anim?.SetTrigger("Death");
            if (AfterDeathPrefab != null)
            {
                Instantiate(AfterDeathPrefab, transform.position, Quaternion.identity);
            }
            //TODO: Fix Hack later
            sr.enabled = false;
            IsDisabled = true;
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            StartCoroutine(Respawn());
        }
    
        public IEnumerator Respawn()
        {
            yield return new WaitForSeconds(RespawnTime);
            //TODO: Fix Hack later
            sr.enabled = true;
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            CurrentHealth = MaxHealth;
            transform.position = StartingPosition;
            currentCombatType = CombatType.None;
            IsDisabled = false;
        }
    }
}
