using System.Collections;
using UnityEngine;

public class Enemy_Boss : GenericEnemy
{
    public GameObject projectile;
    public float projectileInterval;
    public int projetcileCount;
    public int projectileDirectionCount;
    public float abilityCd;
    public float abilityCdDeviation;
    public float dashForce = 5;


    private bool dash = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        base.Start();
        StartCoroutine(AbilitySelect());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!dash) {
            base.Update();
        }
    }

    protected override void FixedUpdate()
    {
        if (dash)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * dashForce, ForceMode2D.Impulse);
            StartCoroutine(resetDash());
        } else {
            base.FixedUpdate();
        }
    }

    IEnumerator resetDash() {
        yield return new WaitForSeconds(1.2f);
        dash = false;
        Rigidbody2D rb2D = gameObject.GetComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().AddForce(-rb2D.linearVelocity * rb.mass, ForceMode2D.Impulse);
    }

    IEnumerator AbilitySelect()
    {
        while (true) {
            int help = Random.Range(0, 100);
            Debug.Log("random number " + help);
            if (help < 65)
            {
                StartCoroutine(AbilityShoot());
            } else
            {
                dash = true;
            }

            yield return new WaitForSeconds(abilityCd + Random.Range(-abilityCdDeviation, abilityCdDeviation));
        }
    }

    IEnumerator AbilityShoot()
    {
        float originalSpeed = speed;
        speed = 0;

        for(int i = 0; i < projetcileCount; i++)
        {
            for(int j = 0; j < projectileDirectionCount; j++)
            {
                Instantiate(projectile, transform.position, Quaternion.AngleAxis((float)((360.0 / projectileDirectionCount * j) + (i * 7.5)), Vector3.forward));
            }

            yield return new WaitForSeconds(projectileInterval);
        }

        speed = originalSpeed;
    }

    protected override void Die()
    {
        UIManager.GameoverScreen(true);
    }
}
