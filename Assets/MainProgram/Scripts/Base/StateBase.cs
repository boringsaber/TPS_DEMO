using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase
{
    /// <summary>
    /// 初始化状态
    /// </summary>
    public abstract void Init(IStateMachineOwner owner);
    /// <summary>
    /// 进入状态
    /// </summary>
    public abstract void Enter();
    /// <summary>
    /// 退出状态
    /// </summary>
    public abstract void Exit();
    /// <summary>
    /// 销毁状态
    /// </summary>
    public abstract void Destory();

    public abstract void Update();

}
