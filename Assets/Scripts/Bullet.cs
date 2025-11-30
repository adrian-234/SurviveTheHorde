using System;
using UnityEngine;

public class Bullet : GenericDamage
{
    public float range; //Milyen messze tud menni a lovedek
    public float speed; //Milyen gyors a lovedek
    /*public float damage; //Mennyit sebez a lovedek(ezt a player script allitja be amikor letrehozza a lovedeket)*/
    public Vector3 startPos; //Az a pozicio ahonnan kilottek a lovedeket

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if ((startPos - transform.position).magnitude >= range)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
