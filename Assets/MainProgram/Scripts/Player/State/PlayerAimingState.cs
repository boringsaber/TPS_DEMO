using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : PlayerStateBase
{
    private int aimingXHash;
    private int aimingYHash;
    private float aimingX = 0;
    private float aimingY = 0;
    private float transitionSpeed = 5;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        aimingXHash = Animator.StringToHash("AimingX");
        aimingYHash = Animator.StringToHash("AimingY");
    }
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayStateAnimation("Aiming");
        if (IsBeControl())
        {
            UpdateAimingTarget();
            playerController.EnterAim();
            
        }
           
    }
    public override void Update()
    {
        base.Update();

        if (IsBeControl()/*&&GameManager.Instance.IsBackPackOpen==false*/)
        {
            playerModel.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            UpdateAimingTarget();
            #region 退出瞄准监听
            if (!playerController.isAiming && !playerController.isFire)
            {
                playerModel.SwitchPlayerState(PlayerState.Idle);
                return;
            }
            #endregion

            #region 开火监听
            if (playerController.isFire)
            {
                playerModel.weapon.Fire(playerController.AimTarget.position);
                playerController.ShakeCamera();
            }
            #endregion

            #region 处理移动输入
            aimingX = Mathf.Lerp(aimingX, playerController.moveInput.x, transitionSpeed * Time.deltaTime);
            aimingY = Mathf.Lerp(aimingY, playerController.moveInput.y, transitionSpeed * Time.deltaTime);
            playerModel.animator.SetFloat(aimingXHash, aimingX);
            playerModel.animator.SetFloat(aimingYHash, aimingY);
            #endregion
        }
    }
    public override void Exit()
    {
        base.Exit();
        if (IsBeControl())
        {
            //playerModel.rig.weight = 0;
            playerController.ExitAim();
        }
    }
    /// <summary>
    /// 从屏幕中心发射射线确认瞄准位置
    /// </summary>
    private void UpdateAimingTarget()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerController.maxRayDistance, playerController.aimLayerMask))
        {
            playerController.AimTarget.position = hit.point;
        }
        else
        {
            playerController.AimTarget.position = ray.origin + ray.direction * playerController.maxRayDistance;
        }
    }
}
