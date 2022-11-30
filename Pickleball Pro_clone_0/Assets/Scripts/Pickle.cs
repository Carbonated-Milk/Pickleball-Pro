using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickle : MonoBehaviour
{
    [SerializeField]
    protected GameObject particalSystem;
    protected float damage = 5;
    [HideInInspector] public Rigidbody rb;
    private bool detonated = false;
    public abstract void Action();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        detonated = true;
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMain>().Damage = 5; //adds 5 to damage
            //nearRB.AddExplosionForce(500 + player.damage, transform.position, 5f, 3f); fix later
        }
        Action();
    }
    public void RandRotate(float multiplier)
    {
        Vector3 randVector = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        rb.angularVelocity = randVector * multiplier;
    }

    public void SpawnParticals()
    {
        GameObject newSystem = Instantiate(particalSystem);
        newSystem.transform.position = transform.position;
        Destroy(newSystem, 5f);
    }
}
