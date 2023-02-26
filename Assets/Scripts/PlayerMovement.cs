using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerActions actions;
    private CharacterController _controller;

    private Vector3 _directionMove;
    readonly private float gravitation = -9.8f;
    private float _sprintSpeed = 1.0f;

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float _playerY = 0.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;
    [SerializeField] private float _jumpForce = 3.0f;


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

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void MovePlayer()
    {
        Vector2 move = actions.Movement.Move.ReadValue<Vector2>();
        _directionMove.x = move.x;
        _directionMove.z = move.y;

        _controller.Move(speed * _sprintSpeed * Time.deltaTime * _directionMove);
    }

    private void ApplyGravity()
    {
        if(_controller.isGrounded && _playerY < 0.0f)
        {
            _playerY = -1.0f;
        }
        else
        {
            _playerY += gravitation * gravityMultiplier * Time.deltaTime;
        }

        _directionMove.y = _playerY;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_controller.isGrounded) return;

        _playerY += _jumpForce;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        print(context.action.IsPressed());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        print("Started " + context.started);
        print("Ended " + context.canceled);
    }
}
