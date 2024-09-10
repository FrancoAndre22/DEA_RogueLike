using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public EnemyType type;
    public float speed = 6;
    public float hp = 2f;
    public float timeToDestroy = 10;
    public float damage = 1f;
    public float bulletSpeed = 3;
    public int bulletsCount = 8;
    public float timeBtwShoot = 1f;
    float timer = 0f;
    bool targetInRange = false;
    public LayerMask collisionsLayerMask;
    public float range = 7f;
    [Header("References")]
    public Transform firePoint;
    public Bullet bulletPrefab;
    Transform target;
    public RoomInitializer myRoom = null;


    private void OnDestroy()
    {
        myRoom.EnemyKilled();
        if(target != null )
            target.GetComponent<Player>().targets.Remove(this);
    }

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        speed = Random.Range(2f, 4f);
        
    }

    void Update()
    {
        switch (type)
        {
            case EnemyType.Normal:
                MoveForward();
                break;
            case EnemyType.RadialShoot:
                MoveForward();
                RadialShoot();
                break;
            case EnemyType.Sniper:
                if (targetInRange)
                {
                    RotateToTarget();
                    Shoot();
                }
                else
                {
                    SearchTarget();
                    MoveForward();
                }
                break;
            case EnemyType.Kamikase:
                if (targetInRange)
                {
                    RotateToTarget();
                    MoveForward(2);
                }
                else
                {
                    SearchTarget();
                    MoveForward();
                }
                break;
            case EnemyType.ArcShoot:
                if (targetInRange)
                {
                    RotateToTarget();
                    ArcShoot();
                }
                else
                {
                    SearchTarget();
                    MoveForward();
                }
                break;
        }
        if (!targetInRange)
        {
            if (CheckObstacles())
            {
                RotateToTarget();
                //Rotate180();
            }
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void MoveForward(float m)
    {
        transform.Translate(Vector3.forward * speed * m * Time.deltaTime);
    }

    void SearchTarget()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= range)
            {
                targetInRange = true;
            }
        }
    }

    bool CheckObstacles()
    {
        RaycastHit hit;
        Physics.Raycast(firePoint.position, firePoint.forward,out hit, 2f, collisionsLayerMask);
        Debug.DrawRay(firePoint.position, firePoint.forward * hit.distance, Color.red);
        if (hit.collider)
        {
            //Debug.Log("encontro algo! " + hit.collider.name);
            return true;
        }
        return false;
    }

    void Rotate180()
    {
        transform.rotation = Quaternion.Euler(0f, 360f - transform.eulerAngles.y + Random.Range(-90,90), 0f);
    }

    void RotateToTarget()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            float angleY = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, -angleY, 0);
        }
    }

    void Shoot()
    {
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            Bullet b = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            b.damage = damage;
            b.speed = bulletSpeed;
        }
    }

    void ArcShoot()
    {
        float arcLenght = 20;
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            float angleAmount = arcLenght / bulletsCount;
            for (int i = 0; i < bulletsCount; i++)
            {
                float angleY = i * angleAmount + transform.eulerAngles.y -(arcLenght / 2f);
                Bullet b = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, angleY, 0));
                b.damage = damage;
                b.speed = bulletSpeed;
            }
        }
    }

    void RadialShoot()
    {
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            float angleAmount = 360f / bulletsCount;
            for (int i = 0; i < bulletsCount; i++)
            {
                float angleY = i * angleAmount;
                Bullet b = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, angleY, 0));
                b.damage = damage;
                b.speed = bulletSpeed;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float dmg)
    {
        hp-= dmg;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}

public enum EnemyType
{
    Normal,
    RadialShoot,
    Sniper,
    Kamikase,
    ArcShoot
}