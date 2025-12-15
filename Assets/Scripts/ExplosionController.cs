using UnityEngine;
using System.Collections;

public class ExplosionController : GenericDamage
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn() {
        yield return new WaitForSeconds(0.25f);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
