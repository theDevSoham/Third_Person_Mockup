using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerActions actions;


    private void Awake()
    {
        actions = new PlayerActions();
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.Movement.Jump.started += OnJump;
        actions.Movement.Shoot.started += OnShoot;
        actions.Movement.Sprint.started += OnSprint;
    }

    private void OnDisable()
    {
        actions.Disable();
        actions.Movement.Jump.started -= OnJump;
        actions.Movement.Shoot.started -= OnShoot;
        actions.Movement.Sprint.started -= OnSprint;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = actions.Movement.Move.ReadValue<Vector2>();
        print(move);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        print(context.action.IsPressed());
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        print(context.action.IsPressed());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        print(context.action.IsPressed());
    }
}
