using UnityEngine;

public class Player_AbilityDash : GenericPlayer
{
    public float dashRange;
    public float dashSpeed;
    public float dashDamage;

    private bool dash = false;
    private Vector3 startPos;
    private Vector3 targetPos;
    private int oldPushbackForce; 


    public override void Ability()
    {
        if (!abilityOnCd)
        {
            abilityOnCd = true;

            startPos = transform.position;
            targetPos = crosshair.transform.position;

            oldPushbackForce = pushbackForce;
            pushbackForce = 30;
            invincible = true;
            dash = true;
        }
    }

    void FixedUpdate()
    {
        if (dash)
        {
            if (Vector3.Distance(startPos, transform.position) < dashRange)
            {
                transform.Translate((targetPos - startPos).normalized * dashSpeed, Space.World);
            } else
            {
                pushbackForce = oldPushbackForce;
                invincible = false;
                dash = false;

                StartCoroutine(ResetAbilityCd());
            }
        } else {
            //karakter mozgatasa es forgatasa input alapjan
            rb2D.linearVelocity = (1 + speed_bonus) * speed_base * movement;

            if (movement.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            } else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {  
        base.OnCollisionEnter2D(other);

        GenericEnemy enemy = other.collider.GetComponent<GenericEnemy>();
        if (dash && enemy)
        {
            enemy.TakeDamage((1 + damage_bonus) * dashDamage);
        }
    }
}
