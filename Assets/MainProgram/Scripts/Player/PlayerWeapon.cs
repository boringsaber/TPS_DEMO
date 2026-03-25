using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWeapon : MonoBehaviour
{
    [Tooltip("子弹生成的位置")]
    public Transform bulletSpawnPoint;
    [Tooltip("子弹预制体")]
    public PlayerWeapnBullet bulletEffectPrefab;
    [Tooltip("枪管火花预制体")]
    public GameObject bulletSparkPrefab;
    [Tooltip("子弹发射间隔")]
    public float bulletInterval = 0.15f;
    private float lastFireTime;//上一次子弹发射时间

    public void Fire(Vector3 targetPos)
    {
        
        if (Time.time - lastFireTime < bulletInterval)
            return;
        lastFireTime = Time.time;
        Vector3 direction = targetPos - bulletSpawnPoint.position;
        direction.Normalize();
        PlayerWeapnBullet bulletEffect = Instantiate(bulletEffectPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bulletEffect.transform.forward = direction;
    }
}
