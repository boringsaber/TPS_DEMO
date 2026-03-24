using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateBase
{
    protected PlayerController playerController;
    protected PlayerModel playerModel;
    public override void Init(IStateMachineOwner owner)
    {
        playerController = PlayerController.Instance;
        playerModel = (PlayerModel)owner;
    }
    public override void Destory()
    {
        
    }

    public override void Enter()
    {
        MonoManager.Instance.AddUpdateAction(Update);
    }

    public override void Exit()
    {
        MonoManager.Instance.RemoveUpdateAction(Update);
    }

    public override void Update()
    {
        #region ÷ÿ¡¶º∆À„
        if (!playerModel.cc.isGrounded)
        {
            playerModel.VerticalSpeed += playerModel.gravity * Time.deltaTime;
            if(playerModel.IsHover())
            playerModel.SwitchPlayerState(PlayerState.Hover);
        }
        else
            playerModel.VerticalSpeed = playerModel.gravity * Time.deltaTime;
        #endregion

        if (playerController.isAiming||playerController.isFire)
            playerModel.SwitchPlayerState(PlayerState.Aiming);
    }
    public bool IsBeControl()
    {
        return playerModel == playerController.currentPlayerModel;
    }
    public void SwitchToHover()
    {
        playerModel.VerticalSpeed = Mathf.Sqrt(-2 * playerModel.gravity * playerModel.Jumpheight);
        playerModel.SwitchPlayerState(PlayerState.Hover);
    }


}
