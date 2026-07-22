using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class GenericWeapon : MonoBehaviour
{
    // [SerializeField]
    // protected Image weaponImage;
    [SerializeField]
    protected float fireRate;
    [SerializeField]
    protected float reloadSpeed;
    [SerializeField]
    protected int ammoCapacity;
    [SerializeField]
    protected GameObject projectile;

    [NonSerialized]
    public GenericPlayer player;

    protected int currentAmmo;
    protected GameObject crosshair;

    protected virtual void Start()
    {
        // itt majd belesz allítva a kep(mondjuk itt lehet nem megoldhato)
        currentAmmo = ammoCapacity;
        crosshair = GameObject.Find("Crosshair");

        StartCoroutine(Shoot());
    }

    public abstract IEnumerator Shoot();
}
