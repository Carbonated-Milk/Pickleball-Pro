using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExplodePickle : Pickle
{
    public float explosionPower;
    public float explosionRadius;

    private void Start()
    {
        RandRotate(3);
    }
    public override void Action()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var nearbyObject in hitColliders)
        {
            Rigidbody nearRB = nearbyObject.GetComponent<Rigidbody>();
            if(nearbyObject.transform.CompareTag("Player"))
            {
                
            }
            if(nearRB != null)
            {
                nearRB.AddExplosionForce(explosionPower, transform.position, explosionRadius, 3f);
            }
            SpawnParticals();
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
