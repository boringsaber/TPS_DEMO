using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieAttackState : EnemyStateBase
{
    private float currentTime = 0;
    private float MaxTime = 3;
    public override void Enter()
    {
        base.Enter();
        enemyModel.PlayStateAnimation("Attack");
    }

    public override void Update()
    {
        base.Update();
        if (EnemyBase.isAttack ==true)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= MaxTime)
            {
                currentTime = 0;
                enemyModel.SwitchState(EnemyState.Idle);
                EnemyBase.isAttack = false;
            }
        }
    }
}
