using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGun : Gun
{
    public GameObject chargePickle;
    public float speed;
    private GameObject pickleSpace;
    private float power;

    public override void Shoot(Transform netObj)
    {
        pickleSpace = Instantiate(chargePickle);
        pickleSpace.GetComponent<Collider>().enabled = false;
        StartCoroutine(ChargeUp(pickleSpace, netObj));
    }
    public override void Release(Transform netObj)
    {
        StopAllCoroutines();
        pickleSpace.GetComponent<Collider>().enabled = true;
        pickleSpace.GetComponent<Rigidbody>().velocity = netObj.forward * power * speed;
        Reset();
    }

    private void Reset()
    {
        pickleSpace = null;
    }

    public IEnumerator ChargeUp(GameObject projectile, Transform netObj)
    {
        power = 0;

        while(true)
        {
            power += Time.deltaTime;
            pickleSpace.transform.position = netObj.position + netObj.forward * 2;
            yield return null;
        }
    }
}
