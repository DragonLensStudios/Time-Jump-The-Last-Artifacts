using System.Collections;
using System.Collections.Generic;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Player;
using PXE.Core.SerializableTypes;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;
using PXE.Example_Games.Don_t_Follow_Me.Scripts.Messages; // Ensure correct namespace usage
using UnityEngine;
using UnityEngine.InputSystem;

public class ICTSJ_PlayerController : PlayerController
{
    [field: SerializeField] public virtual float RunSpeed { get; set; } = 6f;
    [field: SerializeField] public virtual float MinJumpForce { get; set; } = 2f;  // Minimum jump force for initial jump
    [field: SerializeField] public virtual float MaxJumpForce { get; set; } = 7f;  // Maximum jump force for initial jump
    [field: SerializeField] public virtual float MultiJumpForce { get; set; } = 5f;  // Constant force for multi-jumps
    [field: SerializeField] public virtual float MaxJumpTime { get; set; } = 0.5f;  // Time to reach maximum force for initial jump
    [field: SerializeField] public virtual int JumpsAllowed { get; set; } = 2;
    [field: SerializeField] public virtual Transform GroundCheckTransform { get; set; }
    [field: SerializeField] public virtual float GroundCheckRadius { get; set; } = 0.3f;
    [field: SerializeField] public virtual LayerMask GroundLayer { get; set; }
    [field: SerializeField] public virtual LayerMask ColliderObjectLayer { get; set; }
    [field: SerializeField] public virtual float CollisionCheckRadius { get; set; } = 0.5f;
    [field: SerializeField] public virtual float PhaseTime { get; set; } = 0.5f;
    [field: SerializeField] public virtual Color PhaseColor { get; set; }
    [field: SerializeField] public virtual float TransparencyLerpTime { get; set; } = 0.1f; // Time to transition to transparency
    [field: SerializeField] public virtual bool IsPhasing { get; set; } = false;

    protected int currentJumps = 0;
    protected bool isGrounded = false;
    protected bool jumpButtonHeld = false;
    protected float jumpTimer = 0f;
    protected bool isRunning = false;
    protected Color originalColor;

    public override void Start()
    {
        base.Start();
        originalColor = sr.color;
    }


    public override void OnActive()
    {
        base.OnActive();
        playerInput.Player.Jump.performed += JumpOnperformed;
        playerInput.Player.Jump.canceled += JumpOncanceled;
        playerInput.Player.Run.canceled += RunOncanceled;
        playerInput.Player.Run.performed += RunOnperformed;
        playerInput.Player.Special.performed += SpecialOnperformed;
        MessageSystem.MessageManager.RegisterForChannel<ICTSJ_EssenceMessage>(MessageChannels.Player, EssenceMessageHandler);
        MessageSystem.MessageManager.RegisterForChannel<TargetDamageMessage>(MessageChannels.Player, TargetDamageMessageHandler);
    }

    private void TargetDamageMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<TargetDamageMessage>().HasValue) return;
        var data = message.Message<TargetDamageMessage>().GetValueOrDefault();
        if (data.ID != ID) return;
        TakeDamage(data.Damage);
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new ICTSJ_EssenceMessage(ID, CurrentHealth, MaxHealth, Operator.Set));
        
    }

    public virtual void EssenceMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if (!message.Message<ICTSJ_EssenceMessage>().HasValue) return;
        var data = message.Message<ICTSJ_EssenceMessage>().GetValueOrDefault();
        MaxHealth = data.MaxEssencevalue;
        switch (data.Operator)
        {
            case Operator.Add:
                CurrentHealth += data.EssenceValue;
                if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
                break;
            case Operator.Subtract:
                CurrentHealth -= data.EssenceValue;
                break;
            case Operator.Multiply:
                CurrentHealth *= data.EssenceValue;
                if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
                break;
            case Operator.Divide:
                if (data.EssenceValue == 0) return;
                CurrentHealth /= data.EssenceValue;
                break;
            case Operator.Set:
                CurrentHealth = data.EssenceValue;
                if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
                break;
        }
    }

    public override void OnInactive()
    {
        base.OnInactive();
        playerInput.Player.Jump.performed -= JumpOnperformed;
        playerInput.Player.Jump.canceled -= JumpOncanceled;
        playerInput.Player.Run.canceled -= RunOncanceled;
        playerInput.Player.Run.performed -= RunOnperformed;
        playerInput.Player.Special.performed -= SpecialOnperformed;
        MessageSystem.MessageManager.UnregisterForChannel<ICTSJ_EssenceMessage>(MessageChannels.Player, EssenceMessageHandler);
        MessageSystem.MessageManager.UnregisterForChannel<TargetDamageMessage>(MessageChannels.Player, TargetDamageMessageHandler);


    }

    public override void FixedUpdate()
    {
        if (IsDisabled) return;

        isGrounded = Physics2D.OverlapCircle(GroundCheckTransform.position, GroundCheckRadius, GroundLayer);

        if (isGrounded)
        {
            currentJumps = 0;
            jumpTimer = 0f;
        }

        if (jumpButtonHeld && currentJumps == 0 && jumpTimer < MaxJumpTime)
        {
            jumpTimer += Time.fixedDeltaTime;
            ApplyJumpForce(true);  // Apply force progressively for the first jump
        }
        
        rb.linearVelocity = isRunning ? new Vector2(movement.x * RunSpeed, rb.linearVelocity.y) : new Vector2(movement.x * MoveSpeed, rb.linearVelocity.y);
    }

    public virtual void JumpOnperformed(InputAction.CallbackContext input)
    {
        if (IsDisabled || currentJumps >= JumpsAllowed) return;
        jumpButtonHeld = true;
        jumpTimer = 0f;  // Reset the timer on each new jump
        currentJumps++;
        ApplyJumpForce(currentJumps == 0);
        anim.SetTrigger("Jump");
    }

    public virtual void JumpOncanceled(InputAction.CallbackContext input)
    {
        jumpButtonHeld = false;
    }
    
    public virtual void RunOnperformed(InputAction.CallbackContext obj)
    {
        isRunning = true;
        anim.SetBool("isRunning", true);
    }
    
    public virtual void RunOncanceled(InputAction.CallbackContext obj)
    {
        isRunning = false;
        anim.SetBool("isRunning", false);
    }
    
    public virtual void SpecialOnperformed(InputAction.CallbackContext input)
    {
        if (IsDisabled || IsPhasing) return;
        StartCoroutine(HandlePhasing(PhaseTime));
    }

    protected void ApplyJumpForce(bool isFirstJump)
    {
        if (currentJumps > JumpsAllowed) return;
        float jumpForce;
        if (isFirstJump)
        {
            float timeRatio = Mathf.Clamp(jumpTimer / MaxJumpTime, 0f, 1f);
            jumpForce = Mathf.Lerp(MinJumpForce, MaxJumpForce, timeRatio);
        }
        else
        {
            jumpForce = MultiJumpForce;  // Use constant force for multi-jumps
        }
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset vertical velocity for consistent jumps
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public override void MoveOnperformed(InputAction.CallbackContext input)
    {
        if (IsDisabled) return;
        movement = input.ReadValue<Vector2>();
        anim.SetBool("isMoving", true);
        sr.flipX = movement.x < 0;
    }

    public override void MoveOncanceled(InputAction.CallbackContext input)
    {
        if (IsDisabled) return;
        MovementDirection = movement;
        movement = Vector2.zero;
        anim.SetBool("isMoving", false);
    }
    public virtual IEnumerator HandlePhasing(float timeToPhase = 0.5f)
    {
        IsPhasing = true;
        anim.SetBool("isPhasing", IsPhasing);
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new ICTSJ_PhaseMessage(ID, IsPhasing));
        float elapsedTime = 0f;

        // Transition to PhaseColor
        while (elapsedTime < TransparencyLerpTime)
        {
            sr.color = Color.Lerp(originalColor, PhaseColor, elapsedTime / TransparencyLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sr.color = PhaseColor; // Ensure final color is set

        // Wait for the duration of the phase minus the transition time
        yield return new WaitForSeconds(timeToPhase - TransparencyLerpTime);

        // Transition back to original color
        elapsedTime = 0f;
        while (elapsedTime < TransparencyLerpTime)
        {
            sr.color = Color.Lerp(PhaseColor, originalColor, elapsedTime / TransparencyLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sr.color = originalColor; // Ensure final color is set
        IsPhasing = false;
        anim.SetBool("isPhasing", IsPhasing);
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new ICTSJ_PhaseMessage(ID, IsPhasing));
        // Check for collisions and push the player out if necessary
        ResolveCollisions();
    }
    public virtual void ResolveCollisions()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, CollisionCheckRadius, ObstacleLayerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider != GetComponent<Collider2D>())
            {
                Vector2 colliderCenter = hitCollider.bounds.center;
                Vector2 direction;
    
                // Determine whether to push the player left or right based on their position relative to the collider
                if (transform.position.x < colliderCenter.x)
                {
                    direction = Vector2.left;
                }
                else
                {
                    direction = Vector2.right;
                }
    
                // Move the player out of the collider
                transform.position = (Vector2)transform.position + direction;
            }
        }
    }

    public override void OnDie()
    {
        if (anim != null)
        {
            anim?.SetTrigger("Death");
        }
        if (AfterDeathPrefab != null)
        {
            Instantiate(AfterDeathPrefab, transform.position, Quaternion.identity);
        }
        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GameOverState>()));
    }
    
    /// <summary>
    ///  Handles the LevelResetMessage functionality and resets the player.
    /// </summary>
    /// <param name="message"></param>
    public override void LevelResetMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if (!message.Message<LevelResetMessage>().HasValue) return;
        if(gameObject == null) return;
        base.LevelResetMessageHandler(message);
        transform.position = StartingPosition;
        CurrentHealth = MaxHealth;
        anim.ResetTrigger("Death");
        SetObjectActive(true);
    }

    // public virtual void ResolveCollisions()
    // {
    //     Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, CollisionCheckRadius, ObstacleLayerMask);
    //     foreach (var hitCollider in hitColliders)
    //     {
    //         if (hitCollider != null && hitCollider != GetComponent<Collider2D>())
    //         {
    //             Vector2 direction = (Vector2)transform.position - hitCollider.ClosestPoint(transform.position);
    //             transform.position = (Vector2)transform.position + direction.normalized * CollisionCheckRadius;
    //         }
    //     }
    // }
}
