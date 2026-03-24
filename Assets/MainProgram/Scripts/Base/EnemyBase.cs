using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{ 
    Idle,Move,Attack,Dead
}

public abstract class EnemyBase : MonoBehaviour,IStateMachineOwner
{
    [HideInInspector]
    public Animator animator;
    protected StateMachine stateMachine;

    public List<GameObject> dropThings=new List<GameObject>();

    #region 寻路相关
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [Tooltip("旋转速度")]
    public float rotationSpeed = 300f;
    [Tooltip("最小攻击距离")]
    public float minAttackDistance = 1f;
    [HideInInspector]
    public PlayerModel attackTarget;
    #endregion

    #region 流血相关预制体
    [Tooltip("喷血溅射特效")]
    public GameObject bloodSmashPrefab;
    [Tooltip("滴血特效")]
    public GameObject bloodDrippingPrefab;
    #endregion

    #region 受击相关
    protected int hitHash;
    protected int moveSpeedHash;
    protected float normalMoveSpeed = 1;
    protected float slowMoveSpeed = 0.1f;
    protected Coroutine recoverSpeedCoroutine;
    #endregion

    #region 攻击相关
    private float Atk = 50f;
    public static bool isAttack=false;
    #endregion

    #region 血条相关
    [Tooltip("生命值")]
    public int health=100;
    private float currentHealth;
    private bool isDead = false;
    [Tooltip("血条预制体")]
    public GameObject healthBarPrefab;
    [Tooltip("血条的位置")]
    public Transform healthBarPos;
    [HideInInspector]
    public GameObject healthBar;//实例化后的血条
    #endregion

    [Tooltip("血条框显示时间")]
    public float healthBarShowTime = 6;
    private float healthBarShow_timer;

    protected virtual void Update()
    {
        if (isDead) return;
        #region 血条相关
        if (healthBarShow_timer < healthBarShowTime)
        {
            healthBar.SetActive(true);
            healthBar.transform.position = healthBarPos.transform.position;
            healthBarShow_timer += Time.deltaTime;
        }
        else
        {
            healthBar.SetActive(false);
        }
        
        #endregion
    }
    protected virtual void Awake()
    {
        stateMachine = new StateMachine(this);
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = minAttackDistance;
        navMeshAgent.angularSpeed = rotationSpeed;
        hitHash = Animator.StringToHash("Hit");
        moveSpeedHash = Animator.StringToHash("MoveSpeed");
        currentHealth = health;
        healthBarShow_timer = healthBarShowTime;
    }
    protected virtual void Start()
    {
        SwitchState(EnemyState.Idle);
        FindAttackTarget();
        #region 实例化血条框
        healthBar = Instantiate(healthBarPrefab, healthBarPos.position, Quaternion.identity);
        healthBar.transform.SetParent(UIManager.Instance.WorldSpaceCanvas.transform);
        #endregion
        isAttack = false;
    }
    public abstract void SwitchState(EnemyState state);
    public void PlayStateAnimation(string animationName, float transition = 0.25f, int layer = 0)
    {
        animator.CrossFadeInFixedTime(animationName, transition, layer);
    }
    /// <summary>
    /// 减速动画移动速度，一段时间后恢复
    /// </summary>
    protected virtual void SlowMoveAnimation()
    {
        animator.SetFloat(moveSpeedHash, slowMoveSpeed);
        if (recoverSpeedCoroutine != null)
            StopCoroutine(recoverSpeedCoroutine);
        recoverSpeedCoroutine= StartCoroutine(RecoverMoveSpeed(0.5f));
    }
    protected IEnumerator RecoverMoveSpeed(float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.SetFloat(moveSpeedHash, normalMoveSpeed);
        recoverSpeedCoroutine = null;
    }

    /// <summary>
    /// 寻找最近的playermodel
    /// </summary>
    public virtual void FindAttackTarget()
    {
        PlayerModel[] playerModels = GameManager.Instance.playerModels;
        if (playerModels != null && playerModels.Length > 0)
        {
            PlayerModel closestPlayer = null;
            float minDistance = float.MaxValue;
            foreach (PlayerModel player in playerModels)
            {
                if (player  != null)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPlayer = player;
                    }
                }
            }
            attackTarget = closestPlayer;
        }
    }
    /// <summary>
    /// 攻击目标
    /// </summary>
    public  void Attack()
    {
        SwitchState(EnemyState.Attack);
        EventCenter.Instance.EventTrigger("玩家血量更新", -Atk);
        isAttack = true;
        
    }
    /// <summary>
    /// 是否存在攻击目标
    /// </summary>
    /// <returns></returns>
    public virtual bool HasAttackTarget()
    {
        return attackTarget != null;
    }
    /// <summary>
    /// 攻击目标是否在攻击范围内
    /// </summary>
    public virtual bool IsAttackTargetInAttackRange()
    {
        if (HasAttackTarget())
        {
            return Vector3.Distance(transform.position, attackTarget.transform.position) < minAttackDistance;
           
        }
        return false;
    }

    /// <summary>
    /// 追逐目标
    /// </summary>
    public virtual void chaseTarget()
    {
        if(HasAttackTarget() )
        {
            navMeshAgent.SetDestination(attackTarget.transform.position);
        }
    }
    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="damageMultiplier"></param>
    public virtual void Hurt(PlayerWeapnBullet bullet, float damageMultiplier = 1)
    {
        #region 受击动画相关
        animator.SetTrigger(hitHash);
        SlowMoveAnimation();
        #endregion

        #region 生成喷血特效
        Vector3 bulletDir = bullet.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(-bulletDir);
        Destroy(Instantiate(bloodSmashPrefab, bullet.transform.position, rotation), 3);
        #endregion

        #region 生成流血滴落特效
        Destroy(Instantiate(bloodDrippingPrefab, transform.position + Vector3.up * 0.1f, Quaternion.Euler(0, 0, 0)));
        #endregion

        #region 血条相关
        currentHealth -= bullet.damage * damageMultiplier;
        if (currentHealth > 0) //受伤
        {

            healthBarShow_timer = 0;
            healthBar.GetComponent<EnemyHealthBarUI>().UpdateHealthBar(currentHealth / health);
        }
        else //死亡
        {
            SwitchState(EnemyState.Dead);
            EventCenter.Instance.EventTrigger("Zombie", this);
            DropBonus();
            navMeshAgent.enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            currentHealth = 0;
            isDead = true;
            Destroy(healthBar);
        }
        #endregion
    }
        public void Clear()
        {
            stateMachine.StopState();
            Destroy(gameObject);
        }
    private void DropBonus()
    {
        int rand = Random.Range(0, dropThings.Count);
        Transform trans = transform;
        GameObject.Instantiate(dropThings[rand], transform);
    }

    }



