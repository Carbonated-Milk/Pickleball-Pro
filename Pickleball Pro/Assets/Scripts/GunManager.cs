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
    void Start()
    {
        cam = ImportantObjs.camera;

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
        gun1.Hold(cam);

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
            gun1.Shoot(transform.GetChild(0));
        }
        else
        {
            gun1.Release(transform.GetChild(0));
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkObject))]

public abstract class Gun : NetworkBehaviour
{
    protected NetworkObject netObj;
    protected Rigidbody rb;
    private Transform hold;

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
    public virtual void Hold(Transform holder)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        ChangeParentServerRpc(0); //AAAAAAAAAAAAAAAAAAAA
        transform.localRotation = Quaternion.identity;
        transform.GetComponent<Collider>().enabled = false;
    }
    public virtual void Drop()
    {
        gameObject.SetActive(true);
        rb.isKinematic = false;
        ChangeParentServerRpc(0);
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

    [ServerRpc(RequireOwnership = false)]
    void ChangeParentServerRpc(ulong clientId)
    {
        if (clientId != 0) {hold = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform; }
        else hold = null;
    }

    private void Update()
    {
        if(hold != null)
        {
            transform.position = hold.position;
            transform.rotation = hold.rotation;
        }
    }
}
