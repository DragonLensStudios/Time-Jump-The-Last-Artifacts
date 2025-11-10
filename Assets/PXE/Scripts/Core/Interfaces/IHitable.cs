using JetBrains.Annotations;
using PXE.Core.Enums;

namespace PXE.Core.Interfaces
{
    public interface IHitable
    {
        bool IsHitable { get; set; }
        bool IsDodgeing { get; set; }
        bool IsInvincible { get; set; }
        bool IsFlying { get; set; }
        int CurrentHealth { get; set; }
        int MaxHealth { get; set; }
        TeamType Team { get; set; }

        bool OnHit([CanBeNull] IGameObject source, HitType hitType, int damage = 0);
        bool OnHit([CanBeNull] IGameObject source, int damage = 0);
        bool OnHit(HitType hitType, int damage = 0);
        bool OnHit(int damage = 0);
        void OnDie();
    }
}