using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        //Ctx.CharacterController.enabled = true;
        Ctx.CurrentMovementY = Ctx.GroundedGravity;
        Ctx.AppliedMovementY = Ctx.GroundedGravity;

    }
 
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
 
    public override void ExitState(){}
 
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
        //if is grounded and pressed jump, switch to jump state
        if(Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jump());
        }

        if(Ctx.isAscendUsed)
        {
            SwitchState(Factory.Ascend());
        }
    }
}
