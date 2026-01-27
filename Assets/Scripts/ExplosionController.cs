using UnityEngine;
using System.Collections;

public class ExplosionController : GenericDamage
{
    public int size;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(size, size, size);
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn() {
        yield return new WaitForSeconds(0.25f);

        Destroy(gameObject);
    }
}
