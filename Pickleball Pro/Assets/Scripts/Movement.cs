using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour
{
    private Rigidbody rb;
    public Transform cam;
    private PlayerInputActions inputActions;

    public float maxSpeed;
    public float speed;
    public float jumpPow;
    public float sensitivity = 1f;
    public Vector2 center;
    private bool grounded = false;



    public LayerMask groundMask;
    public Vector2 groundSphereCheck;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { Destroy(this); }
    }

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(true);

        rb = GetComponent<Rigidbody>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        //inputActions.Player.Movement.performed += Move;
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Mouse.performed += Mouse;
        inputActions.Player.Recenter.started += Recenter;
        inputActions.Player.Shift.started += Speedy;
        inputActions.Player.Shift.canceled += Speedy;
    }

    void Speedy(InputAction.CallbackContext ctx)
    {
        if (ctx.started) speed *= 2; maxSpeed *= 3 / 2;
        if (ctx.canceled) speed /= 2; maxSpeed /= 3 / 2;
    }
    void Jump(InputAction.CallbackContext ctx)
    {
        if (grounded)
        {
            rb.AddForce(jumpPow * Vector3.up);
        }
    }
    void Mouse(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = ctx.ReadValue<Vector2>() - center;
        transform.rotation = Quaternion.Euler(mousePos.x * sensitivity * Vector3.up);
        cam.localRotation = Quaternion.Euler(-Mathf.Clamp(mousePos.y * sensitivity, -80, 80) * Vector3.right);
    }
    void Recenter(InputAction.CallbackContext ctx)
    {
        center = inputActions.Player.Mouse.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        grounded = Physics.CheckSphere(transform.position - groundSphereCheck.x * Vector3.up, groundSphereCheck.y, groundMask);

        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 moveVec = inputVector.x * transform.right + inputVector.y * transform.forward;

        if (grounded)
        {
            rb.velocity = Vector3.Lerp(rb.velocity - rb.velocity.y * Vector3.up, maxSpeed * moveVec, speed / 100) + rb.velocity.y * Vector3.up;
        }
        else
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity - rb.velocity.y * Vector3.up, maxSpeed * moveVec, speed / 100) + rb.velocity.y * Vector3.up;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;
        if (collision.gameObject.CompareTag("Death")) { GetComponent<PlayerMain>().Die(); }
    }

    private void OnCollisionExit(Collision collision)
    {

    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - groundSphereCheck.x * Vector3.up, groundSphereCheck.y);
    }
}
