using System.Collections;
using UnityEngine;

public class GenericEnemy : GenericDamage
{
    public float speed, hp, xp;

    public GameObject xpSprite;

    protected GameObject player;
    protected Rigidbody2D rb;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(speed * Time.deltaTime * (player.transform.position - transform.position).normalized);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            CollectibleController xpDrop = Instantiate(xpSprite, transform.position, xpSprite.transform.rotation).GetComponent<CollectibleController>();
            xpDrop.xp = xp;

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GenericDamage genericDamage = collision.GetComponent<GenericDamage>();
        if (genericDamage && (genericDamage.targetType == GenericDamage.Targets.All || genericDamage.targetType == GenericDamage.Targets.Enemy))
        {
            Destroy(collision.gameObject);

            TakeDamage(genericDamage.damage);
        }
    }

    //Azert kell igy 2 fuggvennyel csinalni mert ha a FragController hivna meg egybol az IEnumerator-os fv-t akkor az megszakadni WaitForSeconds soran, mert a FragController megszunik kozben es igy a fv nem fejezodne be
    public void Freeze(float time)
    {
        float normalSpeed = speed;
        speed = 0;
        StartCoroutine(ResetFreeze(time, normalSpeed));
    }
    IEnumerator ResetFreeze(float time, float speed)
    {
        yield return new WaitForSeconds(time);
        this.speed = speed;
    }
}
