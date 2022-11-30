using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;
using TMPro;

public class PlayerMain : NetworkBehaviour
{
    public GameObject netCamera;
    public GameObject playerMenu;
    public Transform cam;
    private Movement moveScript;
    public int damage;
    public TMP_Text damageText;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { Destroy(GetComponent<Movement>()); Destroy(this); }
        else
        {
            SetUpCamera();
            SetUpScripts();
        }
    }

    private void SetUpCamera()
    {
        cam.GetComponent<Camera>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;
    }
    private void SetUpScripts()
    {
        GetComponent<GunManager>().cam = cam;
        moveScript = GetComponent<Movement>();
        moveScript.cam = cam;
    }

    private void Awake()
    {
        ImportantObjs.camera = cam;
        //cam.GetComponent<NetworkObject>().Spawn();
        ImportantObjs.player = transform;
    }

    internal void Die()
    {
        playerMenu.SetActive(true);
        moveScript.enabled = false;
    }

    public void Respawn()
    {
        playerMenu.SetActive(false);
        moveScript.enabled = true;
        transform.position = Vector3.zero;
    }

    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage += value;
            damageText.text = "Damage " + damage.ToString();
        }
    }
}
