using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapnBullet : MonoBehaviour
{
    [Tooltip("伤害")]
    public int damage = 10;
    [HideInInspector]
    public Rigidbody rb;
    [Tooltip("推力")]
    public float flypower=30f;
    [Tooltip("存活时间")]
    public float lifetime = 10f;

    private Vector3 prevPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.velocity = transform.forward * flypower;//给子弹一个推力
        Destroy(gameObject, lifetime);

        prevPosition = transform.position;
        CheckInitialOverlap();
    }
    private void Update()
    {
        CheckCollison();
        prevPosition = transform.position; 
    }
    void CheckCollison()
    {
        RaycastHit hit;
        Vector3 dir = transform.position - prevPosition;
        float distance = Vector3.Distance(transform.position, prevPosition);
        if (Physics.Raycast(prevPosition, dir.normalized, out hit, distance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                enemy.Hurt(this, 1);
            }
        }
    }

    void CheckInitialOverlap()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            EnemyBase enemy = hitCollider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.Hurt(this, 1);
                Destroy(gameObject);
                return;
            }
        }
    }
}
