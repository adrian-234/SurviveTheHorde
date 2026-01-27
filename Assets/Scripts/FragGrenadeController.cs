using UnityEngine;

public class FragGrenadeController : MonoBehaviour
{
    public float damage_bonus;
    public float range;
    public float speed;
    public GameObject explosionPrefab;
    public float explosionDamage_base;
    public int explosionSize;
    public float explosionKnockback;
    public GameObject fragPrefab;
    public int fragCount;

    public Vector3 targetPos;

    private Vector3 startPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= range || Vector3.Distance(targetPos, transform.position) < 1)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation).GetComponent<ExplosionController>();
            explosion.damage = explosionDamage_base * damage_bonus;
            explosion.knockback = explosionKnockback;
            explosion.size = explosionSize;

            for(int i = 0; i < fragCount; i++) {
                var frag = Instantiate(fragPrefab, transform.position, Random.rotation).GetComponent<Bullet>();
                frag.damage *= damage_bonus;
            }

            Destroy(gameObject);
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
