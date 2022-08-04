using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour
{
    private Rigidbody rb;
    private Transform cam;
    public float speed;
    public float jumpPow;
    public float sensitivity = 1;
    public Vector2 center;
    private PlayerInputActions inputActions;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { Destroy(this);}
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0).transform;

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Movement.performed += Move;
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Mouse.performed += Mouse;
        inputActions.Player.Recenter.started += Recenter;
    }

    void Move(InputAction.CallbackContext ctx)
    {
        /*Vector2 moveVector = speed * ctx.ReadValue<Vector2>();
        rb.AddForce(moveVector.x * transform.right + moveVector.y * transform.forward);*/
    }
    void Jump(InputAction.CallbackContext ctx)
    {
        rb.AddForce(jumpPow * Vector3.up);
    }
    void Mouse(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = ctx.ReadValue<Vector2>() - center;
        transform.rotation = Quaternion.Euler(mousePos.x * sensitivity * Vector3.up);
        cam.localRotation = Quaternion.Euler(-Mathf.Clamp(mousePos.y * sensitivity, -45, 45) * Vector3.right);
    }
    void Recenter(InputAction.CallbackContext ctx)
    {
        center = inputActions.Player.Mouse.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector2 inputVector = speed * inputActions.Player.Movement.ReadValue<Vector2>();
        rb.AddForce((inputVector.x * transform.right + inputVector.y * transform.forward) * Time.fixedDeltaTime * speed);
    }
}
