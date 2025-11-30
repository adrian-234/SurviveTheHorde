using UnityEngine;
using System.Collections;

public abstract class GenericPlayer : MonoBehaviour
{
    //Alap statisztikak (amelyek majd a szazalekos alapon lesznek novelne)
    public float speed_base, firerate_base, reload_base, hp_base, heal_base;

    //Bonusz statisztikak
    public float speed_bonus, firerate_bonus, reload_bonus, damage_bonus, hp_bonus, heal_bonus;

    //Alap statisztikak (amik nem szazalekosan lesznek novelne)
    public float dodge = 0;     //annak az eselye hogy a jatekos megusszon egy sebzest (max: 100)
    public float xpGain = 1;    //szorzo a jatekos altal felvett tapasztalati pontokra 
    public int luck = 0;        //befolyasolja a fejlesztesi valasztasok soran elerheto opciok minoseget es egy picit megnoveli a kiteres eselyet is
    public int maxAmmo;
    public float minReloadTime;   //Minimum ennyi ido szukseges hogy a karakter fegyvere ujra toltson(masodperc)
    public GameObject bulletSprite; 
    public float xp = 0;
    public int currentLevel = 1;
    public float currentHp;

    private GameObject crosshair;
    private int currentAmmo;

    private Rigidbody2D rb2D;
    private Vector2 movement;
    private bool isGameActive;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        currentAmmo = maxAmmo;
        currentHp = hp_base;

        isGameActive = GameObject.Find("GameManager").GetComponent<GameManager>().isGameActive;

        crosshair = GameObject.Find("Crosshair");

        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        movement = new Vector2(hInput, vInput);

        //Ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ability();
        }
    }

    void FixedUpdate()
    {
        //karakter mozgatasa es forgatasa input alapjan
        rb2D.linearVelocity = (1 + speed_bonus) * speed_base * movement;

        if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        } else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    IEnumerator Shoot()
    {
        while (isGameActive)
        {
            if (currentAmmo == 0)
            {
                float reload =  (1 - reload_bonus) * reload_base;
                if (reload < minReloadTime)
                    reload = minReloadTime;

                yield return new WaitForSeconds(reload);

                currentAmmo = maxAmmo;
            } else
            {
                float firerate = (1 - firerate_bonus) * firerate_base;
                if (firerate < 0)
                    firerate = 0.0f;

                yield return new WaitForSeconds(firerate);

            }
            
            Vector2 posDiff = crosshair.transform.position - transform.position;
            float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

            //Debug.Log("Lottem remaining: " + currentAmmo);
            Bullet bullet = Instantiate(bulletSprite, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<Bullet>();
            currentAmmo--;
            bullet.damage *= 1 + damage_bonus;
            bullet.startPos = transform.position;
        }
    }

    public abstract void Ability();

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Debug.Log("GAME OVER");
            //Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GenericDamage genericDamage = collision.GetComponent<GenericDamage>();
        if (genericDamage && (genericDamage.targetType == GenericDamage.Targets.All || genericDamage.targetType == GenericDamage.Targets.Player))
        {
            if (dodge * 0.9 + luck * 0.05 < Random.Range(0f, 100f))
            {
                collision.gameObject.transform.Translate((transform.position - collision.transform.position).normalized * -10);

                TakeDamage(genericDamage.damage);
            } else
            {
                Debug.Log("Dodge volt");
            }

            return;
        }

        if (collision.CompareTag("Collectible"))
        {
            xp += collision.GetComponent<CollectibleController>().xp * xpGain;

            while (xp >= currentLevel * 2) //Ezt ha atiron majd a GameManagerben is at kell irni
            {
                xp -= currentLevel * 2;
                currentLevel++;

                Debug.Log("LEVEL UP");
            }

            Destroy(collision.gameObject);
        }
    }
}
