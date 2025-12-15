using UnityEngine;

[DefaultExecutionOrder(200)]
public class CrosshairController : MonoBehaviour
{
    private bool autoTarget = false;
    private GameObject player;
    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.paused)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                autoTarget = !autoTarget;
            }

            if (!autoTarget)
            {
                Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = worldMousePos;
            } else
            {
                float? minDist = null; 
                GameObject closest = null;

                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
                    if (minDist == null || dist < minDist)
                    {
                        minDist = dist;
                        closest = enemy;
                    }
                }

                if (closest != null)
                {
                    transform.position = closest.transform.position;
                } else { //Hogyha nincs enemy amire lehetne celozni akkor az egeret kovetni a crosshair auto target-nal is
                    Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    transform.position = worldMousePos;
                }
            }
        }
    }
}
