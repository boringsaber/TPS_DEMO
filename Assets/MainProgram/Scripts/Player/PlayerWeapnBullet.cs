using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapnBullet : MonoBehaviour
{
    [Tooltip("伤害")]
    public int damage = 100;
    [HideInInspector]
    public Rigidbody rb;
    [Tooltip("推力")]
    public float flypower=30f;
    [Tooltip("存活时间")]
    public float lifetime = 1.0f;
    private float currentTime = 0f;
    private Vector3 prevPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        currentTime = 0;
    }

    private void Start()
    {


        //rb.velocity = transform.forward * flypower;//给子弹一个推力
        prevPosition = transform.position;
        CheckInitialOverlap();
       
    }
    
    private void Update()
    {
        currentTime +=Time.deltaTime;
       
         if (currentTime >= lifetime)
        {
            currentTime = 0;
            
            BasePool.instance.ReturnPool(gameObject);
            
        }
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
                BasePool.instance.ReturnPool(gameObject);
            }
            else
            {
                BasePool.instance.ReturnPool(gameObject);
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
                BasePool.instance.ReturnPool(gameObject);

                return;
            }
        }
    }
   

}
