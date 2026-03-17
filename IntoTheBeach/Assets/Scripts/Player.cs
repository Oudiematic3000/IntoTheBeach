using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour, PlayerControls.IPLayerActions
{
    public float speed = 5f;
    Vector3 moveInput;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveInput * speed *Time.deltaTime;
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else
        {
            moveInput = Vector3.zero;
        }
    }
}
