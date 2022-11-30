using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Networking;


public class GunManager : NetworkBehaviour
{
    private PlayerInputActions inputActions;
    public Gun gun1;
    public Gun gun2;
    public Transform cam;
    public Transform shootPoint;
    void Start()
    {
        if (!IsOwner) return;
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Leftclick.started += Shoot;
        inputActions.Player.Leftclick.canceled += Shoot;
        inputActions.Player.E.started += PickUpCheck;
        inputActions.Player.Rightclick.started += DropCheck;
    }

    private void Update()
    {
        if(gun1 != null)
        {
            gun1.GetComponent<Transform>().position = shootPoint.position;
            gun1.GetComponent<Transform>().rotation = shootPoint.rotation;
        }
    }
    void Shoot(InputAction.CallbackContext ctx)
    {
        if (gun1 != null)
        {
            ShootServerRpc(ctx.started);
        }
    }
    void PickUpCheck(InputAction.CallbackContext ctx)
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 3f))
        {
            if (hit.collider.GetComponent<Gun>() != null)
            {
                PickUpWeapon(hit.collider.transform);
            }
        }
    }

    void PickUpWeapon(Transform weapon)
    {
        
        if(gun1 != null) 
        { 
            if (gun2 != null)
            {
                DropWeapon(gun2.transform);
            }
            gun2 = gun1;
            gun2.gameObject.SetActive(false);
        }

        gun1 = weapon.GetComponent<Gun>();
        gun1.Hold(cam.GetComponent<NetworkObject>());
        gun1.transform.parent = transform;
        gun1.SetParentServerRpc();

    }

    void DropCheck(InputAction.CallbackContext ctx)
    {
        if (gun1 == null) { return; }
        DropWeapon(gun1.transform);
    }

    void DropWeapon(Transform weapon)
    {
        if (weapon == null) { return; }
        weapon.GetComponent<Gun>().Drop();
        gun1 = null;

        if (gun2 != null) { gun1 = gun2; gun2 = null; gun1.Activate(true); }
    }

    void Swap()
    {
        Gun temp;
        temp = gun1;
        gun1 = gun2;
        gun2 = temp;
        gun1.Activate(true);
        gun2.Activate(false);
    }

    [ServerRpc]
    void ShootServerRpc(bool t)
    {
        ShootTypeClientRpc(t);
    }

    [ClientRpc]
    void ShootTypeClientRpc(bool t)
    {
        if (t)
        {
            gun1.Shoot(cam);
        }
        else
        {
            gun1.Release(cam);
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkObject))]

public abstract class Gun : NetworkBehaviour
{
    protected NetworkObject netObj;
    protected Rigidbody rb;
    //private Transform hold;

    private void Start()
    {
        netObj = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();
    }
    public virtual void Shoot(Transform t)
    {

    }
    public virtual void Release(Transform t)
    {

    }
    public virtual void Hold(NetworkObject holder)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        /*netObj.TrySetParent(holder);
        //ChangeParentServerRpc(holder); //AAAAAAAAAAAAAAAAAAAA
        transform.localRotation = Quaternion.identity;
        transform.position = holder.transform.GetChild(0).position;*/
        transform.GetComponent<Collider>().enabled = false;
    }
    public virtual void Drop()
    {
        gameObject.SetActive(true);
        rb.isKinematic = false;
        transform.parent = null;
        transform.GetComponent<Collider>().enabled = true;
    }

    public void Activate(bool turnOn)
    {
        if(turnOn)
        {
            gameObject.SetActive(true);
        }
        if (!turnOn)
        {
            gameObject.SetActive(false);
        }
    }

    /*[ServerRpc(RequireOwnership = false)]
    void ChangeParentServerRpc(NetworkObject parentObj)
    {
        if (netObj != null) { Debug.Log(netObj.TrySetParent(parentObj)); }
        else hold = null;
    }*/

    private void Update()
    {
        if(netObj == null) { netObj = GetComponent<NetworkObject>(); }
        
    }
    public static Object FindObjectFromInstanceID(int iid)
    {
        return (Object)typeof(Object)
                .GetMethod("FindObjectFromInstanceID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { iid });

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetParentServerRpc()
    {
        ShootTypeClientRpc();
    }

    [ClientRpc]
    void ShootTypeClientRpc()
    {
        //transform.parent.GetComponent<GunManager>().gun1 = this;
    }
}
