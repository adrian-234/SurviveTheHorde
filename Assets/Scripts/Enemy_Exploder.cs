using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy_Exploder : GenericEnemy
{
    public int chargeupTime;
    public float detontationDistance;
    public float detontationTime;
    public float explosionDamage;
    public int explosionSize;
    public GameObject explosion;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = gameObject.GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (!animator.GetBool("PlayAnimation") && Vector3.Distance(player.transform.position, transform.position) < detontationDistance)
        {
            animator.SetBool("PlayAnimation", true);
            StartCoroutine(ChargeUp());
        } else if (animator.GetBool("PlayAnimation"))
        {
            Debug.Log("Lealtittom az animaciot itt: " + gameObject.name);
            StopCoroutine(ChargeUp());
            animator.SetBool("PlayAnimation", false);
        }
    }

    IEnumerator ChargeUp()
    {
        yield return new WaitForSeconds(chargeupTime);
        if (Vector3.Distance(player.transform.position, transform.position) < detontationDistance)
        {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        speed = 0;
        yield return new WaitForSeconds(detontationTime);

        ExplosionController explosionInstance = Instantiate(explosion, transform.position, explosion.transform.rotation).GetComponent<ExplosionController>();
        explosionInstance.damage = explosionDamage;
        explosionInstance.size = explosionSize;
        Destroy(gameObject);
    }
}
