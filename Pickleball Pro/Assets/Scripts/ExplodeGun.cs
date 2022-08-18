using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplodeGun : Gun
{
    public GameObject explodePickle;
    public float speed;

    public override void Shoot(Transform netObj)
    {
        GameObject projectile = Instantiate(explodePickle);
        projectile.transform.position = netObj.position + netObj.forward * 2;
        projectile.GetComponent<Rigidbody>().velocity = netObj.forward * speed;
    }
    public override void Release(Transform netObj)
    {
        return;
    }
}
