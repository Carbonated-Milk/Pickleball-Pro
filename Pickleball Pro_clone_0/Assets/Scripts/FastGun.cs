using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastGun : Gun
{
    public GameObject smallPickle;
    public Transform barrel;
    public float speed;
    public float power;
    public float maxRotSpeed;
    private float rotSpeed;

    public override void Shoot(Transform netObj)
    {
        StartCoroutine(CommenseFire(netObj));
        StartCoroutine(SpeedUp());
    }
    public override void Release(Transform netObj)
    {
        StopAllCoroutines();
        StartCoroutine(SlowDown());
    }

    public IEnumerator CommenseFire(Transform netObj)
    {
        float time = Time.time;

        while (true)
        {
            barrel.Rotate(Time.deltaTime * rotSpeed * Vector3.forward);
            if(Time.time - time > 1)
            {
                MakePickle(netObj);
                time = Time.time;
            }

            yield return null;
        }
    }
    private void MakePickle(Transform origin)
    {
        Pickle newPickle = Instantiate(smallPickle).GetComponent<Pickle>();
        newPickle.GetComponent<Rigidbody>().velocity = origin.forward * power * speed;
        newPickle.transform.position = origin.position + origin.forward * 2;
    }

    public IEnumerator SpeedUp()
    {

        while(rotSpeed < maxRotSpeed)
        {
            rotSpeed += Time.deltaTime * 20;
            yield return null;
        }
    }
    public IEnumerator SlowDown()
    {
        while (rotSpeed > maxRotSpeed)
        {
            barrel.Rotate(Time.deltaTime * rotSpeed * Vector3.forward);
            rotSpeed -= Time.deltaTime;
            yield return null;
        }
    }
}
