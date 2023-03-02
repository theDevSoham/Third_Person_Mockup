using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerActions actions;
    private CharacterController _controller;

    private Vector3 _directionMove;
    readonly private float gravitation = -9.8f;
    private Transform mainCam;
    public LayerMask hitLayer;
    [SerializeField] private CinemachineVirtualCamera AimVCam;
    [SerializeField] private Sprite[] crosshair;
    [SerializeField] private Image render_Crosshair;
    [SerializeField] private GameObject bullet_hole;

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float _playerY = 0.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;
    [SerializeField] private float _jumpForce = 3.0f;
    [SerializeField] private float _sprintSpeed = 2.0f;
    [SerializeField] private float _rotationSpeed = 1.0f;


    private void Awake()
    {
        actions = new PlayerActions();
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.Movement.Jump.started += OnJump;
        actions.Movement.Shoot.started += OnShoot;
        actions.Movement.Aim.performed += Aim;
        actions.Movement.Aim.canceled += CancelAim;
    }

    private void OnDisable()
    {
        actions.Disable();
        actions.Movement.Jump.started -= OnJump;
        actions.Movement.Shoot.started -= OnShoot;
        actions.Movement.Aim.performed -= Aim;
        actions.Movement.Aim.canceled -= CancelAim;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controller = GetComponent<CharacterController>();
        mainCam = Camera.main.transform;
        if (crosshair[1] != null)
        {
            render_Crosshair.sprite = crosshair[1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotatePlayer();
        ApplyGravity();
    }

    private void MovePlayer()
    {
        Vector2 move = actions.Movement.Move.ReadValue<Vector2>();

        _directionMove = SprintTrigger(move.x) * mainCam.right.normalized + SprintTrigger(move.y) * mainCam.forward.normalized;
        _directionMove.y = _playerY;

        _controller.Move(speed * Time.deltaTime * _directionMove);
    }

    private float SprintTrigger(float movement)
    {
        if (!_controller.isGrounded) return movement; 

        float sprintVal = actions.Movement.Sprint.ReadValue<float>();

        if (sprintVal > 0)
        {
            return movement * _sprintSpeed;
        }
        else
        {
            return movement;
        }
    }

    private void RotatePlayer()
    {
        if (!actions.Movement.Move.IsInProgress()) return;
        RotationFunction();
    }

    private void RotationFunction()
    {
        Quaternion targetAngle = Quaternion.Euler(new Vector3(0, mainCam.eulerAngles.y, 0));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, _rotationSpeed);
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
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_controller.isGrounded) return;

        _playerY += _jumpForce;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        //print(context.action.IsInProgress());

        if (Physics.Raycast(mainCam.position, mainCam.forward, out RaycastHit hit, Mathf.Infinity, hitLayer))
        {
            print(hit.point);
            Vector3 offset = new(0.1f, 0.1f, 0.1f);
            GameObject bulletHoleObject = Instantiate(bullet_hole, hit.point, Quaternion.LookRotation(hit.normal), hit.collider.transform);
            bulletHoleObject.transform.position += bulletHoleObject.transform.forward / 1000;
            Destroy(bulletHoleObject, 2.0f);
        }
    }

    public void Aim(InputAction.CallbackContext context)
    {
        AimVCam.Priority += 10;
        if(crosshair[0] != null)
        {
            render_Crosshair.sprite = crosshair[0];
        }
        RotationFunction();
    }

    public void CancelAim(InputAction.CallbackContext context)
    {
        AimVCam.Priority -= 10;
        if (crosshair[1] != null)
        {
            render_Crosshair.sprite = crosshair[1];
        }
    }
}
