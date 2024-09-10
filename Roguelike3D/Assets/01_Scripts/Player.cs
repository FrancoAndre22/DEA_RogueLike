using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [Header("Stats")]
    public float speed = 4;
    Vector2 dir = Vector2.zero;
    public float shield = 2;
    public float hp = 3;
    public float timer = 0;
    public float timeBtwShoot = 0.5f;
    public float damage = 1f;
    public float bulletSpeed = 5;
    bool canShoot = true;
    bool shooting = false;
    [Header("References")]
    public List<Enemy> targets = new List<Enemy>();
    public Rigidbody rb;
    public Bullet bulletPrefab;
    public Transform firePoint;
    PlayerControls inputs;

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void Awake()
    {
        inputs = new PlayerControls();
        inputs.PlayerMovement.Movement.performed += ctx => dir = ctx.ReadValue<Vector2>();
        inputs.PlayerMovement.Movement.canceled += ctx => dir = Vector2.zero;
        inputs.PlayerMovement.Shoot.started += ctx => shooting = true;
        inputs.PlayerMovement.Shoot.canceled += ctx => shooting = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        Move();
        if (targets.Count > 0)
        {
            RotateToTarget();
        }
        else
        {
            RotateToDir();
        }
        CheckIfCanShoot();
        Shoot();
    }

    void Move()
    {
        //float x = Input.GetAxis("Horizontal");
        //float y = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(dir.x * speed, 0, dir.y * speed);
    }

    void RotateToDir()
    {
        if (dir.magnitude > 0)
        {
            float angleY = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg - 0;
            transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
    }

    void RotateToTarget()
    {
        if (targets[0] != null)
        {
            Vector3 targetDir = targets[0].transform.position - transform.position;
            float angleY = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg - 0;
            transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
    }

    void Shoot()
    {
        if (shooting)
        {
            if (canShoot)
            {
                canShoot = false;
                Bullet b = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
                b.damage = damage;
                b.speed = bulletSpeed;
            }
        }
    }

    void CheckIfCanShoot()
    {
        if (!canShoot)
        {
            timer += Time.deltaTime;
            if (timer >= timeBtwShoot)
            {
                canShoot = true;
                timer = 0;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        hp-= dmg;
        if (hp <= 0)
        {
            SceneManager.LoadScene("Game");
            //Destroy(gameObject);
        }
    }

    public void IncreaseAttackSpeed(float amount, float duration)
    {
        timeBtwShoot += amount;
        StartCoroutine(RemoveBuffAfterDuration(() => timeBtwShoot -= amount, duration));
    }

    public void IncreaseDamage(float amount, float duration)
    {
        damage += amount;
        StartCoroutine(RemoveBuffAfterDuration(() => damage -= amount, duration));
    }

    public void IncreaseHealth(float amount)
    {
        hp += amount;
    }

    public void IncreaseShield(float amount, float duration)
    {
        shield += amount;
        StartCoroutine(RemoveBuffAfterDuration(() => shield -= amount, duration));
    }

    public void IncreaseMovementSpeed(float amount, float duration)
    {
        speed += amount;
        StartCoroutine(RemoveBuffAfterDuration(() => speed -= amount, duration));
    }

    private IEnumerator RemoveBuffAfterDuration(Action removeBuffAction, float duration)
    {
        yield return new WaitForSeconds(duration);
        removeBuffAction();
    }
}