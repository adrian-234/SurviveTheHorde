using UnityEngine;
using System.Collections;

public class StunController : MonoBehaviour
{
    public float stunDuration;

    void Start()
    {
        StartCoroutine(Despawn());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GenericEnemy enemy = collision.GetComponent<GenericEnemy>();
        if (enemy)
        {
            enemy.Freeze(stunDuration);
        }
    }

    IEnumerator Despawn() {
        yield return new WaitForSeconds(0.25f);

        Destroy(gameObject);
    }
}
