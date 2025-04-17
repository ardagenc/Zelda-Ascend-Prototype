using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory){}

    public override void EnterState()
    {
        Ctx.Anim.SetBool("isWalking", true);
        Ctx.Anim.SetBool("isRunning", true);
    }
 
    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * Ctx.runSpeed;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * Ctx.runSpeed;
    }
 
    public override void ExitState(){}
 
    public override void InitializeSubState(){}
 
    public override void CheckSwitchStates()
    {
        if(!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if(Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}
