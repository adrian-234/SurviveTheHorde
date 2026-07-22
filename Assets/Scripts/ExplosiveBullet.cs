using UnityEngine;

public class ExplosiveBullet : Bullet
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int explosionSize;

    protected override void DestroySelf()
    {
        var explosion = Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation).GetComponent<ExplosionController>();
        explosion.damage = damage;
        explosion.SetKnockback(knockback);
        explosion.SetSize(explosionSize);

        Destroy(gameObject);
    }
}
