using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    public float xp;
    public int pickupRange;

    private float speed = 40;
    private static GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= pickupRange) {
            transform.Translate((player.transform.position - transform.position).normalized * speed * Time.deltaTime);
        }
    }
}
