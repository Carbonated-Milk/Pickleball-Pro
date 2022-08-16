using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickle : MonoBehaviour
{
    [SerializeField]
    protected GameObject particalSystem;
    protected Rigidbody rb;
    public abstract void Action();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
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
