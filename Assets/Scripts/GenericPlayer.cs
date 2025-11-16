using UnityEngine;

public abstract class GenericPlayer : MonoBehaviour
{
    //Alap statisztikak (amelyek majd a szazalekos alapon lesznek novelne)
    public float speed_base, firerate_base, reload_base, damage_base, hp_base, heal_base;

    //Bonusz statisztikak
    public float speed_bonus, firerate_bonus, reload_bonus, damage_bonus, hp_bonus, heal_bonus;

    //Alap statisztikak (amik nem szazalekosan lesznek novelne)
    public float dodge = 0;     //annak az eselye hogy a jatekos megusszon egy sebzest (max: 100)
    public float xpGain = 1;    //szorzo a jatekos altal felvett tapasztalati pontokra 
    public int luck = 0;        //befolyasolja a fejlesztesi valasztasok soran elerheto opciok minoseget
    public int maxAmmo;

    private int currentAmmo;

    private Rigidbody2D rb2D;
    private Vector2 movement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
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

    public abstract void Ability();
}
