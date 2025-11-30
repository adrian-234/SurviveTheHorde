using UnityEngine;

public class GenericEnemy : GenericDamage
{
    public float speed, hp, xp;

    public GameObject xpSprite;

    private GameObject player;
    private Rigidbody2D rb;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = (player.transform.position - transform.position).normalized * speed;
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
}
