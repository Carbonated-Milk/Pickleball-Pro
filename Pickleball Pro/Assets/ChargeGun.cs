using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGun : Gun
{
    public GameObject chargePickle;
    public float speed;
    private Pickle pickleSpace;
    private float power;

    public override void Shoot(Transform netObj)
    {
        pickleSpace = Instantiate(chargePickle).GetComponent<Pickle>();
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

    public IEnumerator ChargeUp(Pickle projectile, Transform netObj)
    {
        power = 0;

        while(true)
        {
            power += Time.deltaTime;
            pickleSpace.rb.angularVelocity = pickleSpace.rb.angularVelocity.normalized * (pickleSpace.rb.angularVelocity.magnitude + 3 * Time.deltaTime);
            pickleSpace.transform.position = netObj.position + netObj.forward * 2;
            yield return null;
        }
    }
}
