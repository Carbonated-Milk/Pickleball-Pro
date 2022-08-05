using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExplodeGun : MonoBehaviour, IGun
{
    public GameObject explodePickle;
    public float speed;

    public void Shoot(Transform netObj)
    {
        GameObject projectile = Instantiate(explodePickle);
        projectile.transform.position = netObj.position + netObj.forward * 2;
        projectile.GetComponent<Rigidbody>().velocity = netObj.forward * speed;
    }
    public void Release(Transform netObj)
    {
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
