using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.Projectiles
{
    public class HitableProjectile : BaseProjectile
    {
        [field: SerializeField] public virtual int Damage { get; set; } = 1;
        [field: SerializeField] public virtual HitType HitType { get; set; } = HitType.None;
        [field: SerializeField] public virtual ObjectController AfterHitPrefab { get; set; }

        [field: SerializeField] public virtual float ReflectDamageMultiplier { get; set; } = 1.0f;

        public bool Dying = false;

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger) return;
            if (other.gameObject == null) return;
            if (other.gameObject == gameObject) return;
            var target = other.gameObject.GetComponent<IHitable>();
            if (target == null) return;
            if (Dying) return;
            if (target.OnHit(this, HitType, Damage))
            {
                Dying = true;
                if (AfterHitPrefab != null) Instantiate(AfterHitPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
                return;
            }
        }
    }
}