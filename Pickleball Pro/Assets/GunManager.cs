using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class GunManager : NetworkBehaviour
{
    private PlayerInputActions inputActions;
    public Gun gun1;
    public Gun gun2;
    public Transform cam;
    void Start()
    {
        cam = ImportantObjs.camera;
        Debug.Log(cam);

        if (!IsOwner) return;
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Leftclick.started += Shoot;
        inputActions.Player.Leftclick.canceled += Shoot;
        inputActions.Player.E.started += PickUpCheck;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Shoot(InputAction.CallbackContext ctx)
    {
        if(gun1 != null)
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
        if(gun2 != null) { DropWeapon(gun2.transform); }

        gun2 = gun1;
        gun1 = weapon.GetComponent<Gun>();
        gun2.gameObject.SetActive(false);

        weapon.parent = cam;
        weapon.position = cam.GetChild(0).position;
        weapon.GetComponent<Collider>().enabled = false;
    }

    void DropWeapon(Transform weapon)
    {
        weapon.parent = null;
        weapon.position = cam.GetChild(0).position;
        weapon.GetComponent<Collider>().enabled = true;

        if(gun2 != null) { gun1 = gun2; }
    }

    [ServerRpc]
    void ShootServerRpc(bool t)
    {
        ShootTypeClientRpc(t);
    }

    [ClientRpc]
    void ShootTypeClientRpc(bool t)
    {
        if(t)
        {
            gun1.Shoot(transform.GetChild(0));
        }
        else
        {
            gun1.Release(transform.GetChild(0));
        }
    }
}

public abstract class Gun: MonoBehaviour
{
    public virtual void Shoot(Transform t)
    {

    }
    public virtual void Release(Transform t)
    {

    }
}
