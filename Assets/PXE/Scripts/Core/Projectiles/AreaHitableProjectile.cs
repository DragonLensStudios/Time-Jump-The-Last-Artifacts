using PXE.Core.Interfaces;
using UnityEngine;

namespace PXE.Core.Projectiles
{
    public class AreaHitableProjectile : HitableProjectile
    {
        protected override void OnTriggerEnter2D(Collider2D other) {}

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject == null) return;
            if (other.gameObject == gameObject) return;
            var target = other.gameObject.GetComponent<IHitable>();
            if (target == null) return;
            if (target.OnHit(this, Damage))
            {
                if (AfterHitPrefab != null) Instantiate(AfterHitPrefab, transform.position, transform.rotation);
            }
        }
    }
}