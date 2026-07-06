using UnityEngine;
using System.Collections;

public class ExplosionController : GenericDamage
{
    private int size;    //A robbanas hatosugara
    private float knockback; //Mennyivel tud vissza lokni az robbanas
    [SerializeField]
    private float despawnTime;
    private float growthRate;

    public void SetSize(int s)
    {
        size = s;
        growthRate = size / despawnTime;
    }

    public void SetKnockback(float k)
    {
        knockback = k;
    }

    void Start()
    {
        StartCoroutine(Despawn());
    }

    void Update()
    {
        if (transform.localScale.x < size)
        {
            transform.localScale += new Vector3(growthRate * Time.deltaTime, growthRate * Time.deltaTime);
        }
    }

    IEnumerator Despawn() {
        yield return new WaitForSeconds(despawnTime);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.GetComponent<GenericEnemy>().hp > damage)
        {
            Vector3 direction = (collision.transform.position - transform.position).normalized;
            collision.transform.position += knockback * direction;
        }
    }
}
