using System.Collections;
using UnityEngine;

public class Weapon_Basic : GenericWeapon
{
    public override IEnumerator Shoot()
    {
        while(true)
        {
            float shootCd;
            if (currentAmmo == 0)
            {
                shootCd = reloadSpeed * player.GetReloadBonus() / 100.0f;

                currentAmmo = ammoCapacity + player.GetAmmoCapacityBonus();
            } else
            {
                shootCd = fireRate * player.GetFirerateBonus() / 100.0f; 
            }
            yield return new WaitForSeconds(shootCd);
            
            Vector2 posDiff = crosshair.transform.position - transform.position;
            float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

            Bullet bullet = Instantiate(projectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<Bullet>();
            currentAmmo--;
            bullet.damage *= player.GetDamageBonus() / 100.0f;
            bullet.penetration += player.GetAmmoPenetrationBonus();
        }
    }
}
