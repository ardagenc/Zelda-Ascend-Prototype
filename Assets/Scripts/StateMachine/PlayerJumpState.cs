using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        HandleJump();
    }
 
    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleGravity();
    }
 
    public override void ExitState()
    {
        Ctx.IsJumping = false;
        if(Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
        Ctx.Anim.SetBool("isJumping", false);
    }
 
    public override void InitializeSubState()
    {
        if(!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if(Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else if(Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
    }
 
    public override void CheckSwitchStates()
    {
        //another solution for isGrounded bool var.
        //bool raycast = Physics.Raycast(Ctx.transform.position + Vector3.up, Vector3.down, 1.1f);

        if(Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }
    
    void HandleJump()
    {
        Ctx.Anim.SetBool("isJumping", true);
        Ctx.IsJumping = true;
        Ctx.CurrentMovementY = Ctx.JumpVelocity;
        Ctx.AppliedMovementY = Ctx.JumpVelocity;
        
    }

    void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;

        if(isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Ctx.FallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) / 2;
        }
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) / 2;
        }
    }
}
