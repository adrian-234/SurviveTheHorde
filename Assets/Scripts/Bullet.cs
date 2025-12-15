using System;
using UnityEngine;

public class Bullet : GenericDamage
{
    public float range; //Milyen messze tud menni a lovedek
    public float speed; //Milyen gyors a lovedek
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

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
