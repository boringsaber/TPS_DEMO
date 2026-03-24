using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private int moveBlendHash;
    private float moveBlend;
    private float walkThreshold = 0;//行走阈值
    private float runThreshold = 1;//奔跑阈值
    private float transitionSpeed = 5;//过渡速度

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        moveBlendHash = Animator.StringToHash("MoveBlend");
    }
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayStateAnimation("Move");
    }
    public override void Update()
    {
        base.Update();
        if (IsBeControl())
        {
            #region 悬空状态监听
            if (playerController.isJumping)
            {
                SwitchToHover();
                return;
            }
            #endregion
            if (playerController.moveInput.magnitude == 0)
            {
                playerModel.SwitchPlayerState(PlayerState.Idle);
                return;
            }
            if (playerController.isSprint)
            {
                moveBlend = Mathf.Lerp(moveBlend, runThreshold, transitionSpeed * Time.deltaTime);
            }
            else
            {
                moveBlend = Mathf.Lerp(moveBlend, walkThreshold, transitionSpeed * Time.deltaTime);
            }
            playerModel.animator.SetFloat(moveBlendHash, moveBlend);

            float rad = Mathf.Atan2(playerController.localMovement.x, playerController.localMovement.z);
            playerModel.transform.Rotate(0, rad * playerController.rotationSpeed * Time.deltaTime, 0);
        }
    }
}
