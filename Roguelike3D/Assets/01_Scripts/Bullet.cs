using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 7;
    public float damage = 1f;
    public Rigidbody rb;
    public float timeToDestroy = 3;
    public bool isPlayerBullet = true;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
        rb.velocity = transform.forward * speed;
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isPlayerBullet)
        {
            Enemy e = collision.gameObject.GetComponent<Enemy>();
            e.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player") && !isPlayerBullet)
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TakeDamage(damage);
            Destroy(gameObject);
        }
        //Destroy(gameObject);
    }
}
