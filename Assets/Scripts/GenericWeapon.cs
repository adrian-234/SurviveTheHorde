using System;
using System.Collections;
using UnityEngine;
public abstract class GenericWeapon : MonoBehaviour
{
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
        currentAmmo = ammoCapacity;
        crosshair = GameObject.Find("Crosshair");

        StartCoroutine(Shoot());
    }

    public abstract IEnumerator Shoot();
}
