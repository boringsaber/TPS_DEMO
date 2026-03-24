using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum PlayerState
{ 
    Idle,Move,Hover,Aiming
}
public class PlayerModel : MonoBehaviour,IStateMachineOwner
{
    [Tooltip("角色武器")]
    public PlayerWeapon weapon;

    [HideInInspector]
    public Animator animator;
    private StateMachine stateMachine;
    private PlayerState currentState;
    public CharacterController cc;
    //public Rig rig;

    [Tooltip("重力")]
    public float gravity = -15;
    [Tooltip("跳跃高度")]
    public float Jumpheight = 1.5f;
    [HideInInspector]
    public float VerticalSpeed;
    public float fallHeight = 0.2f;

    private static readonly int CACHE_SIZE = 3;
    Vector3[] SpeedCache = new Vector3[CACHE_SIZE];
    private int SpeedCache_index = 0;
    private Vector3 averageDeltaMovement;

    public MultiAimConstraint rightHandAimConstraint;//瞄准状态下的右手约束
    public TwoBoneIKConstraint rightHandConstraint;//正常状态右手约束
    public MultiAimConstraint bodyAimConstraint;//身躯约束

    #region 血量相关
    
    #endregion
    private void Awake()
    {
        stateMachine = new StateMachine(this);
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }
    private void Start()
    {
        SwitchPlayerState(PlayerState.Idle);
        EventCenter.Instance.AddEventListener("Zombie", (info) => {
            if (info is EnemyBase)
            {
                print("获得1金钱");
            }
        });
    }
    private void Update()
    {
        
    }
    public void SwitchPlayerState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                stateMachine.EnterState<PlayerIdleState>();
                break;
            case PlayerState.Move:
                stateMachine.EnterState<PlayerMoveState>();
                break;
            case PlayerState.Hover:
                stateMachine.EnterState<PlayerHoverState>();
                break;
            case PlayerState.Aiming:
                stateMachine.EnterState<PlayerAimingState>();
                break;
        }
        currentState = state;
    }

    public void PlayStateAnimation(string animationName, float transition = 0.25f, int layer = 0)
    { 
        animator.CrossFadeInFixedTime(animationName,transition,layer);
    }
    private void OnAnimatorMove()
    {
        Vector3 playerDeltaMovement = animator.deltaPosition;//获取动画控制器当前帧的位置信息
        if (currentState != PlayerState.Hover)
        {
            UpdateAverageCacheSpeed(animator.velocity);
        }
        else
        {
            playerDeltaMovement = averageDeltaMovement*Time.deltaTime;
        }
        playerDeltaMovement.y = VerticalSpeed * Time.deltaTime;
        cc.Move(playerDeltaMovement);
    }

    private void UpdateAverageCacheSpeed(Vector3 newSpeed)
    {
        SpeedCache[SpeedCache_index++] = newSpeed;
        SpeedCache_index %= CACHE_SIZE;
        Vector3 sum = Vector3.zero;//计算缓存中的平均速度
        foreach (Vector3 cache in SpeedCache)
            sum += cache;
        averageDeltaMovement = sum / CACHE_SIZE;
    }

    public bool IsHover()
    {
        return !Physics.Raycast(transform.position, Vector3.down, fallHeight);
    }
}
