using System.Collections;
using UnityEngine;

/*
    kepessegek:
        spiral loves:
           - 6 helyen lő (60 fokonként)
           - 8szor lő egy sorozatban
           - minden lövésnél 7.5 fokkal odébb forog a lövési pont
           - lövés közben teljesen megáll
           - 65% esélyel lesz ez az ability használva
        dash:
            - dashhel egy nagyott a player fele
            - akkár túl is lőhet azon
            - 35% esélyel lesz ez az ability használva
        ?gyors robbbantósokat spawnol maga alá?
    
    lassan mozog
    nagy darab

    sűrűbben lő mint dasshel
*/

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
        if (dash)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * dashForce, ForceMode2D.Impulse);

            dash = false;
        } else
        {
            base.Update();
        }
    }

    IEnumerator AbilitySelect()
    {
        while (true) {
            if (Random.Range(0, 100) < 65)
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
        gameManager.GameoverScreen(true);
    }
}
