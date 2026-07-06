using UnityEngine;

public class StunGrenade : MonoBehaviour
{
    public float range;
    public float speed;
    public int size;
    public float stunDuration;
    public GameObject explosionPrefab;

    public Vector3 targetPos;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) >= range || Vector3.Distance(targetPos, transform.position) < 1)
        {
            StunController obj = Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation).GetComponent<StunController>();
            obj.SetSize(size);
            obj.SetStunDuration(stunDuration);

            Destroy(gameObject);
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
