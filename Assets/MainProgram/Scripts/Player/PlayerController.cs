using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;



public class PlayerController : SingleMonoBase<PlayerController>
{
    public PlayerModel currentPlayerModel;
    private Transform cameraTransform;
    public GameObject MyBag;
    private bool isOpen = true;
    
    [Tooltip("转向速度")]
    public float rotationSpeed=300;

    #region 玩家输入相关
    private MyInputSystem input;
    [HideInInspector]
    public Vector2 moveInput;
    [HideInInspector]
    public bool isSprint;//冲刺输入
    [HideInInspector]
    public bool isAiming;//瞄准输入
    [HideInInspector]
    public bool isJumping;//跳跃输入
    [HideInInspector]
    public bool isFire;//开火输入
    [HideInInspector]
    public bool isBackPackOpen;//背包打开输入
    [Tooltip("正常视角相机")]
    public CinemachineFreeLook freeLookCamera;
    [Tooltip("瞄准视角相机")]
    public CinemachineFreeLook aimingCamera;
    #endregion
    [HideInInspector]
    public Vector3 localMovement;
    [HideInInspector]
    public Vector3 worldMovement;

    #region 瞄准相关
    
    [Tooltip("瞄准目标")]
    public Transform AimTarget;
    [Tooltip("射线检测的最大距离")]
    public float maxRayDistance = 1000f;
    [Tooltip("射线检测的层级")]
    public LayerMask aimLayerMask = ~0;
    #endregion

    #region 开火抖动
    private CinemachineImpulseSource impulseSource;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        input = new MyInputSystem();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        ExitAim();
        impulseSource = aimingCamera.GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>().normalized;
        isSprint = input.Player.isSprint.IsPressed();
        isAiming = input.Player.isAiming.IsPressed();
        isJumping = input.Player.isJumping.IsPressed();
        isFire = input.Player.Fire.IsPressed();
        isBackPackOpen = input.Player.isBackPackOpen.IsInProgress();

        Vector3 cameraForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        worldMovement = cameraForwardProjection * moveInput.y + cameraTransform.right * moveInput.x;
        localMovement = currentPlayerModel.transform.InverseTransformVector(worldMovement);
        OpenOrCloseBackpack();
    }

    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    public void EnterAim()
    {
        aimingCamera.m_XAxis.Value = freeLookCamera.m_XAxis.Value;
        aimingCamera.m_YAxis.Value = freeLookCamera.m_YAxis.Value;

        currentPlayerModel.rightHandAimConstraint.weight = 1;
        currentPlayerModel.bodyAimConstraint.weight = 1;
        currentPlayerModel.rightHandConstraint.weight = 0;

        freeLookCamera.Priority = 0;
        aimingCamera.Priority = 100;
    }
    public void ExitAim()
    {
        freeLookCamera.m_XAxis.Value = aimingCamera.m_XAxis.Value;
        freeLookCamera.m_YAxis.Value = aimingCamera.m_YAxis.Value;

        currentPlayerModel.rightHandAimConstraint.weight = 0;
        currentPlayerModel.bodyAimConstraint.weight = 0;
        currentPlayerModel.rightHandConstraint.weight = 1;

        freeLookCamera.Priority = 100;
        aimingCamera.Priority = 0;
    }

   

    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }

    
    /// <summary>
    /// 打开或者关闭背包
    /// </summary>
    private void OpenOrCloseBackpack()
    {
        if (Input.GetKeyDown(KeyCode.B) && isOpen)
        {
            isOpen = false;
            MyBag.SetActive(true);

        }
        else if (Input.GetKeyDown(KeyCode.B) && isOpen == false)
        {
            isOpen = true;
            MyBag.SetActive(false);

        }
    }
}
