using System.Collections;
using UnityEngine;

public class GenericEnemy : GenericDamage
{
    public float speed, hp, xp;

    public GameObject xpSprite;

    protected static GameObject player;
    protected static UIManager UIManager;
    protected Rigidbody2D rb;   
    protected Animator animator;
    protected Vector2 movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = gameObject.GetComponent<Animator>();

        if (player == null || UIManager == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            UIManager = FindFirstObjectByType<UIManager>();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        movement = (player.transform.position - transform.position).normalized;
    }

    protected virtual void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            animator.SetBool("Walk", true);
            if (movement.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            } else if (movement.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        } else
        {
            animator.SetBool("Walk", false);
        }

        //karakter mozgatasa 
        rb.linearVelocity = speed * movement;        
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        CollectibleController xpDrop = Instantiate(xpSprite, transform.position, xpSprite.transform.rotation).GetComponent<CollectibleController>();
        xpDrop.xp = xp;

        UIManager.killCounter++;

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GenericDamage genericDamage = collision.GetComponent<GenericDamage>();
        if (genericDamage && (genericDamage.targetType == Targets.All || genericDamage.targetType == Targets.Enemy))
        {            
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
