using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class GunManager : NetworkBehaviour
{
    private PlayerInputActions inputActions;
    public IGun gun1;
    void Start()
    {
        gun1 = transform.GetChild(1).GetComponent<IGun>();
        if (!IsOwner) return;
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Leftclick.started += Shoot;
        inputActions.Player.Leftclick.canceled += Shoot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot(InputAction.CallbackContext ctx)
    {
        ShootServerRpc(ctx.started);
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

public interface IGun
{
    void Shoot(Transform t);
    void Release(Transform t);
}
