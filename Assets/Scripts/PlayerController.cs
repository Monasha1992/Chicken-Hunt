using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables: Movement

    private GameManager _gameManager;
    private GameObject _body;
    private GameObject _mouth;
    private GameObject _blood;

    public ParticleSystem chickenEatParticles;
    public ParticleSystem foxDieParticles;

    private AudioSource _playerAudio;
    public AudioClip chickenCaughtSound;

    private bool _isDead;

    private Vector2 _input;
    private CharacterController _characterController;
    private Vector3 _direction;

    [SerializeField] private float speed;

    [SerializeField] private Movement movement;

    #endregion

    #region Variables: Rotation

    [SerializeField] private float rotationSpeed = 500f;
    private Camera _mainCamera;

    #endregion

    #region Variables: Gravity

    private float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;

    #endregion

    #region Variables: Jumping

    [SerializeField] private float jumpPower;
    private int _numberOfJumps;
    [SerializeField] private int maxNumberOfJumps = 2;

    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        _gameManager = FindObjectOfType<GameManager>();
        _body = GameObject.Find("Fox");
        _mouth = GameObject.Find("Mouth");
        _blood = GameObject.Find("Blood");
        // _camera = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
        _playerAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        ApplyRotation();
        ApplyGravity();

        if (_gameManager.isGameOver && !_isDead && !_gameManager.hasWon) Died();
        else if (!(_isDead || _gameManager.hasWon)) ApplyMovement();
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        _direction = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f) *
                     new Vector3(_input.x, 0.0f, _input.y);
        var targetRotation = Quaternion.LookRotation(_direction, Vector3.up);

        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        var targetSpeed = movement.isSprinting ? movement.speed * movement.multiplier : movement.speed;
        movement.currentSpeed =
            Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);

        _characterController.Move(_direction * (movement.currentSpeed * Time.deltaTime));
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0.0f, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded() && _numberOfJumps >= maxNumberOfJumps) return;
        if (_numberOfJumps == 0) StartCoroutine(WaitForLanding());

        _numberOfJumps++;
        _velocity = jumpPower;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        movement.isSprinting = context.started || context.performed;
    }

    private IEnumerator WaitForLanding()
    {
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(IsGrounded);

        _numberOfJumps = 0;
    }

    private bool IsGrounded() => _characterController.isGrounded;

    private void Died()
    {
        _isDead = true;
        _gameManager.PlayerDied();
        _body.transform.position = new Vector3(_body.transform.position.x, _body.transform.position.y,
            _body.transform.position.z);
        _mouth.transform.position = new Vector3(_mouth.transform.position.x, 0, _mouth.transform.position.z);
        _body.transform.Rotate(0, 0, 90, Space.Self);
        _mouth.transform.Rotate(0, 0, 0, Space.Self);
        foxDieParticles.Play();
        _blood.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Chicken") && !_gameManager.hasCaughtChicken)
        {
            hit.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            hit.gameObject.transform.position = _mouth.transform.position;
            hit.gameObject.transform.SetParent(_mouth.transform);
            hit.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            _gameManager.CaughtChicken();
            chickenEatParticles.Play();
            _playerAudio.PlayOneShot(chickenCaughtSound, 1.0f);
        }
        else if (hit.gameObject.CompareTag("Finish Line") && !_gameManager.isGameOver)
        {
            _gameManager.ReachedJungle();
        }
    }
}

[Serializable]
public struct Movement
{
    public float speed;
    public float multiplier;
    public float acceleration;

    [HideInInspector] public bool isSprinting;
    [HideInInspector] public float currentSpeed;
}