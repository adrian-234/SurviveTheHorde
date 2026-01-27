using System.Collections;
using UnityEngine;

public class Enemy_Shooter : GenericEnemy
{
    public GameObject projectile;
    public float shootCd;
    public float minPlayerDist;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 targetPos = (transform.position - player.transform.position) / (Vector2.Distance(player.transform.position,transform.position) / minPlayerDist) + player.transform.position;

        transform.Translate(speed * Time.deltaTime * (targetPos - transform.position).normalized);
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            Vector2 posDiff = player.transform.position - transform.position;
            float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

            Instantiate(projectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

            yield return new WaitForSeconds(shootCd);
        }
    }
}
