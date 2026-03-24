using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayStateAnimation("Idle");
    }
    public override void Update()
    {
        base.Update();
        if (IsBeControl())
        { 
            if (playerController.moveInput.magnitude != 0)
            playerModel.SwitchPlayerState(PlayerState.Move);

            #region Ðü¿Õ×´Ì¬¼àÌý
            if (playerController.isJumping)
                SwitchToHover();
            #endregion
        }


    }
}
