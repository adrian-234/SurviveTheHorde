using UnityEngine;
using System.Collections;
using System;

public abstract class GenericPlayer : MonoBehaviour
{
    //Alap statisztikak (amelyek majd a szazalekos alapon lesznek novelne)
    [SerializeField]
    protected float speed_base, /*firerate_base, reload_base,*/ hp_base, heal_base;

    //Bonusz statisztikak
    protected int speed_bonus = 100, firerate_bonus = 100, reload_bonus = 100, damage_bonus = 100, hp_bonus = 100, heal_bonus = 100;

    //Alap statisztikak (amik nem szazalekosan lesznek novelne)
    [SerializeField]
    protected int dodge = 0;     //annak az eselye hogy a jatekos megusszon egy sebzest (max: 100)
    protected float xpGain = 1;    //szorzo a jatekos altal felvett tapasztalati pontokra 
    protected int luck = 0;        //befolyasolja a fejlesztesi valasztasok soran elerheto opciok minoseget es egy picit megnoveli a kiteres eselyet is
    protected int ammoCapacityBonus = 0, ammoPenetrationBonus = 0; 

    protected int maxAmmo; // TODO majd ki kell torolni

    public float GetHp()
    {
        return hp_base * hp_bonus / 100f;
    }
    public int GetHpBonus()
    {
        return hp_bonus;
    }
    public void AddHpBonus(int bonus)
    {
        hp_bonus += bonus;
    }

    public float GetSpeed()
    {
        return speed_base * speed_bonus / 100f;
    }
    public int GetSpeedBonus()
    {
        return speed_bonus;
    }
    public void AddSpeedBonus(int bonus)
    {
        speed_bonus += bonus;
    }

    public float GetFirerateBonus()
    {
        return firerate_bonus;
    }
    public void AddFirerateBonus(int bonus)
    {
        firerate_bonus -= bonus;
    }

    public float GetReloadBonus()
    {
        return reload_bonus;
    }
    public void AddReloadBonus(int bonus)
    {
        reload_bonus -= bonus;
    }

    public float GetHeal()
    {
        return heal_base * heal_bonus / 100f;
    }
    public int GetHealBonus()
    {
        return heal_bonus;
    }
    public void AddHealBonus(int bonus)
    {
        heal_bonus += bonus;
    }

    public int GetDamageBonus()
    {
        return damage_bonus;
    }
    public void AddDamageBonus(int bonus)
    {
        damage_bonus += bonus;
    }

    public int GetDodge()
    {
        return dodge;
    }
    public void AddDodgeBonus(int bonus)
    {
        dodge += bonus;
        if (dodge > 100)
        {
            dodge = 100;
        }
    }

    public float GetXpGain()
    {
        return xpGain;
    }
    public void AddXpGainBonus(float bonus)
    {
        xpGain += bonus / 100;
    }

    public int GetLuck()
    {
        return luck;
    }
    public void AddLuckBonus(int bonus)
    {
        luck += bonus;
    }

    public int GetAmmoCapacityBonus()
    {
        return ammoCapacityBonus;
    }
    public void AddAmmoCapacityBonus(int bonus)
    {
        ammoCapacityBonus += bonus;
    }

    public int GetAmmoPenetrationBonus()
    {
        return ammoPenetrationBonus;
    }
    public void AddAmmoPenetraionBonus(int bonus)
    {
        ammoPenetrationBonus += bonus;
    }

    public void AddBoostByName(string statName, int bonus)
    {
        switch(statName.ToLower())
        {
            case "damage":
                AddDamageBonus(bonus);
                break;
            case "health":
                AddHpBonus(bonus);
                break;
            case "move speed":
                AddSpeedBonus(bonus);
                break;
            case "fire rate":
                AddFirerateBonus(bonus);
                break;
            case "reload speed":
                AddReloadBonus(bonus);
                break;
            case "heal":
                AddHealBonus(bonus);
                break;
            case "dodge":
                AddDodgeBonus(bonus);
                break;
            case "xp gain":
                AddXpGainBonus(bonus);
                break;
            case "luck":
                AddLuckBonus(bonus);
                break;
            case "ammo capacity":
                AddAmmoCapacityBonus(bonus);
                break;
            case "ammo penetration":
                AddAmmoPenetraionBonus(bonus);
                break;
            default:
                Debug.LogError("Nem letezeo player statisztika kerult novelesre. Keresett statisztika: " + statName);
                break;
        }
    }

    [SerializeField]
    protected int pushbackForce = 10;

    public GameObject bulletSprite; 
    public float currentXp = 0;
    public int currentLevel = 1;
    public float currentHp;
    public float abilityCdTime;
    public bool abilityOnCd = false;
    public GenericWeapon weapon;

    [SerializeField]
    private GameObject dodgeTextPrefab;

    protected GameObject crosshair;
    private int currentAmmo;
    protected bool invincible = false;
    protected Rigidbody2D rb2D;
    protected Vector2 movement;
    protected GameManager gameManager;
    protected UIManager UIManager;
    private Animator animator;
    private Transform gameCanvas;
    private static readonly int dodgeTextSpawnRadius = 8;
    private static readonly WaitForSeconds HealCd = new(1);


    void Start()
    {
        UIManager = FindFirstObjectByType<UIManager>();
        UIManager.SetPlayer(this);

        rb2D = GetComponent<Rigidbody2D>(); // TODO mindegyik csak GetComponent legyen ne gameoject.getcomponent és mindenhez legyen requirement is
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        crosshair = GameObject.Find("Crosshair");
        gameCanvas = GameObject.Find("GameCanvas").transform;
        
        currentAmmo = maxAmmo;
        currentHp = GetHp();

        // StartCoroutine(Shoot());
        StartCoroutine(Heal());
    }

    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        movement = new Vector2(hInput, vInput);
        if (movement != Vector2.zero)
        {
            animator.SetBool("Walk", true);
        } else
        {
            animator.SetBool("Walk", false);
        }

        //Ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ability();
            UIManager.abilityCounter++;
        }
    }

    void FixedUpdate()
    {
        //karakter mozgatasa es forgatasa input alapjan
        rb2D.linearVelocity = GetSpeed() * movement;

        if (movement.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        } else if (movement.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // IEnumerator Shoot()
    // {
    //     while (true)
    //     {
    //         if (currentAmmo == 0)
    //         {
    //             float reload = GetReload();
    //             yield return new WaitForSeconds(reload);

    //             currentAmmo = maxAmmo;
    //         } else
    //         {
    //             yield return new WaitForSeconds(GetFirerate());
    //         }
            
    //         Vector2 posDiff = crosshair.transform.position - transform.position;
    //         float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

    //         Bullet bullet = Instantiate(bulletSprite, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<Bullet>();
    //         currentAmmo--;
    //         bullet.damage *= GetDamageBonus() / 100.0f;
    //     }
    // }

    IEnumerator Heal() {
        while (true) {
            float heal = GetHeal();
            float maxHp = GetHp();

            if (currentHp + heal > maxHp) {
                currentHp = maxHp;
            } else {
                currentHp += heal;
            }

            yield return HealCd;
        }
    }

    public abstract void Ability();

    protected IEnumerator ResetAbilityCd() {
        yield return new WaitForSeconds(abilityCdTime);

        abilityOnCd = false;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            UIManager.GameoverScreen(false);
        }
    }

    protected void SpawnDodgeText()
    {
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randX = (float)Math.Cos(angle) * dodgeTextSpawnRadius;
        float randY = (float)Math.Sin(angle) * dodgeTextSpawnRadius;

        GameObject text = Instantiate(dodgeTextPrefab, gameCanvas);
        text.transform.position = transform.position + new Vector3(randX, randY, 0);
    }

    public int XpNeededForLvlUp()
    {
        return currentLevel * 2;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.collider);
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider);
    }

    protected void HandleCollision(Collider2D collider)
    {
        GenericDamage genericDamage = collider.GetComponent<GenericDamage>();
        if (genericDamage && (genericDamage.targetType == GenericDamage.Targets.All || genericDamage.targetType == GenericDamage.Targets.Player))
        {
            collider.gameObject.transform.Translate(-1 * pushbackForce * (transform.position - collider.transform.position).normalized);
            if (!invincible)
            {
                if (dodge * 0.9 + luck * 0.1 < UnityEngine.Random.Range(0, 100))
                {
                    TakeDamage(genericDamage.damage);
                } else
                {
                    SpawnDodgeText();
                }
            }
        } else if (collider.CompareTag("Collectible"))
        {
            currentXp += collider.GetComponent<CollectibleController>().xp * xpGain;

            while (currentXp >= XpNeededForLvlUp())
            {
                currentXp -= XpNeededForLvlUp();
                currentLevel++;

                UIManager.LevelUpScreen();
            }

            Destroy(collider.gameObject);
        }
    }
}
