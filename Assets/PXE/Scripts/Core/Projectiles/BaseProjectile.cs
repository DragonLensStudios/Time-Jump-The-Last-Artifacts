using PXE.Core.Enums;
using PXE.Core.Interfaces;
using PXE.Core.Objects;
using UnityEngine;

namespace PXE.Core.Projectiles
{
    public abstract class BaseProjectile : ObjectController, IRotateType
    {
        [field: SerializeField] public virtual float Speed { get; set; } = 10f;
        [field: SerializeField] public virtual bool HasLifeDuration { get; set; } = true;
        [field: SerializeField] public virtual float LifeDuration { get; set; } = 10f;

        [field: SerializeField] public virtual ObjectController Owner { get; set; }
        [field: SerializeField] public virtual Vector3 MovementDirection { get; set; } = Vector3.up;
    
        [field: SerializeField] public virtual RotateType RotateType { get; set; }

        [field: SerializeField] public virtual float ResizeEffectMultiplier { get; set; } = 1.0f;
        [field: SerializeField] public virtual float LobIdleDuration { get; set; } = 0f;

        [field: SerializeField] public virtual float ReflectSpeedMultiplier { get; set; } = 1.0f;

        [field: SerializeField] public virtual GameObject AfterDurationPrefab { get; set; }
    
        public virtual float LifeCountdown { get; set; } = 10f;
        protected virtual Vector3 BaseScale { get; set; }
    
        public override void Start()
        {
            base.Start();
            BaseScale = transform.localScale;
            LifeCountdown = LifeDuration;
            if (Owner == null)
            {
                MovementDirection = MovementDirection.y >= 0 ? Vector3.up : Vector3.down;
            }

            // //TODO: Why is this here?! 
            // if (Name.Contains("aser"))
            // {
            //     MovementDirection = MovementDirection.x >= 0 ? Vector3.right : Vector3.left;
            // }
            // if (RotateType == RotateType.Parent4Directions || RotateType == RotateType.Moving4Directions || RotateType == RotateType.Target4Directions)
            // {
            //     float degrees = TU.DirectionToDegrees(MovementDirection);
            //     MovementDirection = TU.DegreesToDirection(Mathf.Floor((degrees + 45f) / 90f) * 90f);
            // }
            // if (RotateType == RotateType.Parent8Directions || RotateType == RotateType.Moving8Directions || RotateType == RotateType.Target8Directions)
            // {
            //     float degrees = TU.DirectionToDegrees(MovementDirection);
            //     MovementDirection = TU.DegreesToDirection(Mathf.Floor((degrees + 22.5f) / 45f) * 45f);
            // }
        }

        public override void Update()
        {
            base.Update();
            // this.UpdateRotate(MovementDirection);

            if (HasLifeDuration)
            {
                // this.UpdateRotate(MovementDirection);

                if (HasLifeDuration)
                {
                    LifeCountdown -= UnityEngine.Time.deltaTime;
                    if (LifeCountdown <= 0f)
                    {
                        transform.position += MovementDirection * (Speed * UnityEngine.Time.deltaTime);
                    }

                    if (LobIdleDuration is > 0f or < 0f)
                    {
                        if (LifeCountdown > LobIdleDuration)
                        {
                            transform.position += MovementDirection * (Speed * UnityEngine.Time.deltaTime);
                        }
                        var lobAirDuration = LifeDuration - LobIdleDuration;
                        var lobAirCountdown = LifeCountdown - LobIdleDuration;
                        var lobPercent = Mathf.Clamp(lobAirCountdown / lobAirDuration, 0.0f, 1.0f);
                        var resizeMultiplier = (ResizeEffectMultiplier - 1.0f) * Mathf.Sin(lobPercent * Mathf.PI) + 1.0f;
                        transform.localScale = new Vector3(BaseScale.x * resizeMultiplier, BaseScale.y * resizeMultiplier);

                    }
                    else if (ResizeEffectMultiplier is > 1.0f or < 1.0f)
                    {
                        transform.position += MovementDirection * (Speed * UnityEngine.Time.deltaTime);
                        var resizeMultiplier = (ResizeEffectMultiplier - 1.0f) * (LifeDuration - LifeCountdown) / LifeDuration + 1.0f;
                        transform.localScale = new Vector3(BaseScale.x * resizeMultiplier, BaseScale.y * resizeMultiplier);
                    }
                }
                else if (ResizeEffectMultiplier is > 1.0f or < 1.0f)
                {
                    transform.position += MovementDirection * (Speed * UnityEngine.Time.deltaTime);
                    var resizeMultiplier = (ResizeEffectMultiplier - 1.0f) * (LifeDuration - LifeCountdown) / LifeDuration + 1.0f;
                    transform.localScale = new Vector3(BaseScale.x * resizeMultiplier, BaseScale.y * resizeMultiplier);
                }
            }
        }
    }
}
