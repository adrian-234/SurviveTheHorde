using System;
using UnityEngine;

public class Bullet : GenericDamage
{
    public float range; //Milyen messze tud menni a lovedek
    public float speed; //Milyen gyors a lovedek
    public float knockback; //Mennyivel tud vissza lokni az adott dolog 
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
            Destroy(gameObject);
        }

        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetType == Targets.Enemy || targetType == Targets.All) && collision.CompareTag("Enemy"))
        {
            Vector3 direction = (collision.transform.position - transform.position).normalized;
            collision.transform.position += knockback * direction;

            Destroy(gameObject);
        }

        if ((targetType == Targets.Player || targetType == Targets.All) && collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
