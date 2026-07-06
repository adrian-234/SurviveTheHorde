using TMPro;
using UnityEngine;

public class DodgeTextController : MonoBehaviour
{
    public float DespawnTimeInSec;
    public float floatSpeed;

    private float timer;
    private TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= DespawnTimeInSec)
        {
            Destroy(gameObject);
        } else
        {
            float t = timer / DespawnTimeInSec;

            transform.position += Vector3.up * Time.deltaTime * floatSpeed;
            text.alpha = 1 - t;
        }
    }
}
