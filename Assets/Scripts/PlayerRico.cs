using UnityEngine;

public class PlayerRico : GenericPlayer
{
    public GameObject grenadePrefab;

    public override void Ability()
    {
        if (!abilityOnCd) {
            abilityOnCd = true;

            Vector2 posDiff = crosshair.transform.position - transform.position;
            float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

            var grenade = Instantiate(grenadePrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<FragGrenadeController>();
            grenade.damage_bonus = 1 + damage_bonus;
            grenade.targetPos = crosshair.transform.position;

            StartCoroutine(ResetAbilityCd());
        }
    }
}
