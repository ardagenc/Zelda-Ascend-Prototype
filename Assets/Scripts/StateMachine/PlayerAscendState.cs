using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAscendState : PlayerBaseState
{
    public PlayerAscendState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public PlayerStateMachine playerScript;
    public Transform player;

    public override void EnterState()
    {
        Ctx.cam.m_Priority = 4;
    }
 
    public override void UpdateState()
    {
        CheckSwitchStates();

        if(Ctx.isAscendPressed && !Ctx.stopAscend)
        {   
            Ascend();
        }
        if(Ctx.climbButtonPressed && Ctx.stopAscend)
        {
            AscendClimb();
        }
    }
 
    public override void ExitState()
    {
        Ctx.isAscendPressed = false;
        Ctx.stopAscend = false;
        Ctx.switchGroundState = false;
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
        if(Ctx.switchGroundState)
        {
            SwitchState(Factory.Grounded());
        }
        
        if(!Ctx.isAscendUsed)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public void Ascend()
    {
        Ctx.ascendVFX.EnableRendererFeatures();
        Ctx.ascendParticle.SetActive(true);
        Ctx.ascendSpiralParticle.SetActive(true);
        Ctx.ascendSpiralParticleFlipped.SetActive(true);

        Ctx.cam.m_Priority = 10;
        Ctx.CharacterController.enabled = false;
        Ctx.transform.Translate(Vector3.up * Ctx.ascendSpeed * Time.deltaTime);
        Ctx.Anim.SetBool("isAscending", true);
        
    }

    public void AscendClimb()
    {
        Ctx.climbButtonPressed = false; 
        Ctx.Anim.SetBool("Climb", true);

        Ctx.transform.DOMoveY(Ctx.transform.position.y + 0.9f, 1f);
    }

}
