using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    private GameManager _gameManager;

    private Vector3 _direction;
    private CharacterController _controller;

    private Transform _camera;
    private GameObject _body;
    private GameObject _mouth;
    private GameObject _blood;

    public ParticleSystem chickenEatParticles;
    public ParticleSystem dogDieParticles;

    private AudioSource _playerAudio;
    public AudioClip chickenCaughtSound;

    private const float SmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    public float GravityMultiplier = 1.2f;
    public float JumpPower = 4;
    private float _speed = 1f;
    private float _currentVelocity;
    private float _velocity;
    private bool _isDead;
    public bool isGrounded = true;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _controller = GetComponent<CharacterController>();
        _body = GameObject.Find("Fox");
        _mouth = GameObject.Find("Mouth");
        _blood = GameObject.Find("Blood");
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
        _playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_gameManager.isGameOver && !_isDead && !_gameManager.hasWon) Died();
        else RunSpeed();

        MovePlayerRelativeToCamera();
    }

    private void MovePlayerRelativeToCamera()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    // apply gravity when the player is not grounded
    private void ApplyGravity()
    {
        if (isGrounded)
        {
            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                _velocity += JumpPower;
                isGrounded = false;
            }

            else if (_velocity < 0) _velocity = -1;
        }
        else
        {
            _velocity += Gravity * GravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
        if (_direction.y <= 0 && !isGrounded) isGrounded = true;
        // Debug.Log($" {isGrounded} -- {_velocity}");
    }

    private void ApplyRotation()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        var direction = new Vector3(horizontal, 0, vertical).normalized;
        if (!(direction.magnitude >= 0.1f)) return;

        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity,
            SmoothTime);

        transform.rotation = Quaternion.Euler(0, angle, 0);
        // var moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        // _direction = moveDir;
        _direction = direction;
    }

    // Move player
    private void ApplyMovement()
    {
        if (_gameManager.gameStarted) _controller.Move(_direction.normalized * (_speed * Time.deltaTime));
    }

    private void Died()
    {
        _isDead = true;
        _gameManager.PlayerDied();
        _body.transform.position = new Vector3(_body.transform.position.x, _body.transform.position.y,
            _body.transform.position.z);
        _mouth.transform.position = new Vector3(_mouth.transform.position.x, 0, _mouth.transform.position.z);
        _body.transform.Rotate(0, 0, 90, Space.Self);
        _mouth.transform.Rotate(0, 0, 0, Space.Self);
        dogDieParticles.Play();
        _blood.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    private void RunSpeed()
    {
        // shift to increase speed upto 6 when shift is pressed
        _speed = Input.GetKey(KeyCode.LeftShift) ? 6 : 1;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Chicken") && !_gameManager.hasCaughtChicken)
        {
            hit.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            hit.gameObject.transform.position = _mouth.transform.position;
            hit.gameObject.transform.SetParent(_mouth.transform);
            hit.gameObject.GetComponent<NavMeshAgent>().enabled = false;
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