using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoverState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayStateAnimation("Hover");
    }

    public override void Update()
    {
        base.Update();
        if (playerModel.cc.isGrounded)
            playerModel.SwitchPlayerState(PlayerState.Idle);
    }
}
