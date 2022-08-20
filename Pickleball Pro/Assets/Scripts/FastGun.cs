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
    private float rotSpeed = 0.0001f;

    public override void Shoot(Transform netObj)
    {
        StopAllCoroutines();
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
            barrel.Rotate(10 * Time.deltaTime * rotSpeed * Vector3.right);
            if(Time.time - time > 1 / rotSpeed * 10)
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
        newPickle.GetComponent<Rigidbody>().velocity = power * (speed + rotSpeed/10) * origin.forward;
        newPickle.transform.position = origin.position + origin.forward * 2;
    }

    public IEnumerator SpeedUp()
    {

        while(rotSpeed < maxRotSpeed)
        {
            rotSpeed += Time.deltaTime * 50;
            yield return null;
        }
    }
    public IEnumerator SlowDown()
    {
        while (rotSpeed > 0)
        {
            barrel.Rotate(10 * Time.deltaTime * rotSpeed * Vector3.right);
            rotSpeed -= Time.deltaTime * 100;
            yield return null;
        }
        rotSpeed = 0.0001f;
    }
}
