using UnityEngine;
using System.Collections;

public class StunController : MonoBehaviour
{
    [SerializeField]
    private float despawnTime;

    private float size;      //A robbanas hatosugara
    private float stunDuration;

    private float growthRate;

    public void SetSize(int s)
    {
        size = s;
        growthRate = size / despawnTime;
    }

    public void SetStunDuration(float d)
    {
        stunDuration = d;
    }

    void Start()
    {
        StartCoroutine(Despawn());
    }

    void Update()
    {
        if (transform.localScale.x < size)
        {
            transform.localScale += new Vector3(growthRate * Time.deltaTime, growthRate * Time.deltaTime);
        }
    }

    IEnumerator Despawn() {
        yield return new WaitForSeconds(despawnTime);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GenericEnemy enemy = collision.GetComponent<GenericEnemy>();
        if (enemy)
        {
            enemy.Freeze(stunDuration);
        }
    }
}
