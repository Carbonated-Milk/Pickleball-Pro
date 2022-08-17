using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChargePickle : Pickle
{
    public float explosionPower;
    public float explosionRadius;
    public bool displayRadius;
    public GameObject afterPickle;

    private void Start()
    {
        RandRotate(3);
        StartCoroutine(AfterPickle(afterPickle));
    }
    public override void Action()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var nearbyObject in hitColliders)
        {
            Rigidbody nearRB = nearbyObject.GetComponent<Rigidbody>();
            if (nearbyObject.transform.CompareTag("Player"))
            {

            }
            if (nearRB != null)
            {
                nearRB.AddExplosionForce(explosionPower, transform.position, explosionRadius, 3f);
            }
            SpawnParticals();
            Destroy(gameObject);
        }
    }

    public IEnumerator AfterPickle(GameObject afterPickle)
    {
        float time = Time.time;

        while (true)
        {
            if (Time.time - time > .2)
            {
                Instantiate(afterPickle, transform).gameObject.SetActive(true);
                time = Time.time;
            }
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        if (displayRadius)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, explosionRadius);
        }
    }
}

