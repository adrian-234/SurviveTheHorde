using System.Collections;
using UnityEngine;

public class Enemy_Shooter : GenericEnemy
{
    public GameObject projectile;
    public float shootCd;
    public float minPlayerDist;
    private bool inDeadzone = false; 
    private float deadzone = 0.5f;
    private Coroutine shootCoroutine;
    private WaitForSeconds _shootCd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        _shootCd = new(shootCd);
        shootCoroutine = StartCoroutine(Shoot());
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 targetPos = (transform.position - player.transform.position) / (Vector2.Distance(player.transform.position,transform.position) / minPlayerDist) + player.transform.position;

        inDeadzone = (targetPos - transform.position).magnitude < deadzone;

        // transform.Translate(speed * Time.deltaTime * (targetPos - transform.position).normalized);
        movement = (targetPos - transform.position).normalized;
    }

    protected override void FixedUpdate()
    {
        if (!inDeadzone && movement != Vector2.zero)
        {
            animator.SetBool("Walk", true);
            if (movement.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            } else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        } else
        {
            animator.SetBool("Walk", false);
        }

        //karakter mozgatasa 
        rb.linearVelocity = speed * movement; 
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            Vector2 posDiff = player.transform.position - transform.position;
            float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

            Instantiate(projectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

            yield return _shootCd;
        }
    }

    public override void Freeze(float time)
    {
        StopCoroutine(shootCoroutine);
        base.Freeze(time);
    }

    protected override IEnumerator ResetFreeze(float time, float speed)
    {
        yield return base.ResetFreeze(time, speed);
        shootCoroutine = StartCoroutine(Shoot());
    }
}
