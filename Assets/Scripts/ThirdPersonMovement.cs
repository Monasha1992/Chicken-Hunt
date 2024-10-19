using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    private GameManager _gameManager;

    private CharacterController _controller;
    private Transform _camera;
    private GameObject _body;
    private GameObject _mouth;
    private GameObject _blood;

    public ParticleSystem chickenEatParticles;
    public ParticleSystem dogDieParticles;

    private AudioSource _playerAudio;
    public AudioClip chickenCaughtSound;

    public Vector2 _input;
    public Vector3 _direction;
    public float speed = 1f;

    // public bool isSpeedLocked;
    private const float SmoothTime = 0.1f;
    private float _smoothVelocity;
    private bool _isDead;

    private const float Gravity = -9.81f;
    private const float GravityMultiplier = 1;
    private float _velocity;

    private const float JumpPower = 4;

    private Vector3 _playerVelocity;

    // private float _playerSpeed = 2.0f;
    // private const float JumpHeight = 10f;
    // private const float GravityValue = -9.81f;

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
    void Update()
    {
        if (_gameManager.isGameOver && !_isDead && !_gameManager.hasWon) Died();
        else RunSpeed();

        MovePlayerRelativeToCamera2();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        _direction = new Vector3(_input.x, 0, _input.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed) return;
        if (!_controller.isGrounded) return;

        _velocity += JumpPower;
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // if (speed < 6f)
            // {
            //     if (isSpeedLocked) return;
            //
            //     isSpeedLocked = true;
            //     speed += 0.5f;
            //     Debug.Log($"isSpeedLocked 1 -> {isSpeedLocked}");
            //     StartCoroutine(_gameManager.Delay(5f));
            //     Debug.Log($"isSpeedLocked 2 -> {isSpeedLocked}");
            //     // isSpeedLocked = false;
            // }
            speed = 6;
            // Debug.Log("Running");
        }
        else
        {
            speed = 1;
            // Debug.Log("Stopped");
        }
    }

    private void MovePlayerRelativeToCamera()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        if (Camera.main == null) return;

        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        var forwardRelativeVerticalInput = vertical * forward;
        var rightRelativeHorizontalInput = horizontal * right;

        var cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        transform.Translate(cameraRelativeMovement * (speed * Time.deltaTime), Space.World);
    }

    private void MovePlayerRelativeToCamera2()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
        // var horizontal = Input.GetAxisRaw("Horizontal");
        // var vertical = Input.GetAxisRaw("Vertical");

        // var direction = new Vector3(horizontal, 0, vertical).normalized;


        // if (direction.y < 0) direction.y = 0f;
        // direction.Normalize();
        //
        // // Changes the height position of the player
        // if (Input.GetButtonDown("Jump")) direction.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
        // direction.y += GravityValue * Time.deltaTime;
    }

    // apply gravity when the player is not grounded
    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity < 0)
        {
            _velocity = -1;
        }
        else
        {
            _velocity += Gravity * GravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _smoothVelocity,
            SmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        // _direction = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
    }

    private void ApplyMovement()
    {
        if (_gameManager.gameStarted) _controller.Move(_direction.normalized * (speed * Time.deltaTime));
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