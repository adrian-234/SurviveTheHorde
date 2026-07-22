using System;
using UnityEngine;

public class Bullet : GenericDamage
{
    public float range; //Milyen messze tud menni a lovedek
    public float speed; //Milyen gyors a lovedek
    public float knockback; //Mennyivel tud vissza lokni az adott dolog
    public int penetration;
    private Vector3 startPos; //Az a pozicio ahonnan kilottek a lovedeket

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= range)
        {
            DestroySelf();
        }

        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //player lovedekei
        if ((targetType == Targets.Enemy || targetType == Targets.All) && collision.CompareTag("Enemy"))
        {
            if (--penetration > 0)
            {
                GenericEnemy enemy = collision.GetComponent<GenericEnemy>();
                if (enemy && enemy.hp > damage) {
                    Vector3 knockbackDirection = knockback * (collision.transform.position - transform.position).normalized;
                    collision.transform.Translate(knockbackDirection, Space.World);
                    transform.Translate(knockbackDirection, Space.World);
                }
            } else {
                DestroySelf();
            }
        }

        //Ellenfelek levedekenek torlese ha hozza ernek a playerhez
        if ((targetType == Targets.Player || targetType == Targets.All) && collision.CompareTag("Player"))
        {
            DestroySelf();
        }
    }

    virtual protected void DestroySelf()
    {
        Destroy(gameObject);
    }
}
