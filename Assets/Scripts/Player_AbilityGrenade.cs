using UnityEngine;

public class Player_AbilityGrenade : GenericPlayer
{
    public GameObject grenadePrefab;
    
    public override void Ability()
    {
        if (!abilityOnCd) {
            abilityOnCd = true;

            Vector2 posDiff = crosshair.transform.position - transform.position;
            float angle = Mathf.Atan2(posDiff.y, posDiff.x) * Mathf.Rad2Deg;

            GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

            if (grenade.GetComponent<FragGrenadeController>())
            {
                FragGrenadeController fragGrenade = grenade.GetComponent<FragGrenadeController>();
                fragGrenade.damage_bonus = 1 + damage_bonus;
                fragGrenade.targetPos = crosshair.transform.position;
            } else if (grenade.GetComponent<StunGrenade>())
            {
                StunGrenade stunGrenade = grenade.GetComponent<StunGrenade>();
                stunGrenade.targetPos = crosshair.transform.position;
            }

            StartCoroutine(ResetAbilityCd());
        }
    }
}
