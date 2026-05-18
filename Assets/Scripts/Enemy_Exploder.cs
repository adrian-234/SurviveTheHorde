using System.Collections;
using System.Runtime.InteropServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Enemy_Exploder : GenericEnemy
{
    public int chargeupTime;
    public float detontationDistance;
    public float detontationTime;
    public float explosionDamage;
    public int explosionSize;
    public GameObject explosion;
    private bool chargingUp = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!chargingUp && Vector3.Distance(player.transform.position, transform.position) < detontationDistance)
        {
            chargingUp = true;
            StartCoroutine(ChargeUp());
        } else if (chargingUp && Vector3.Distance(player.transform.position, transform.position) > detontationDistance)
        {
            StopCoroutine(ChargeUp());
            chargingUp = false;
        }
        animator.SetBool("ChargeUp", chargingUp);
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
