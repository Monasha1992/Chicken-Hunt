using UnityEngine;
using UnityEngine.AI;

public class ThirdPersonMovement : MonoBehaviour
{
    private GameManager _gameManager;

    private CharacterController _controller;
    private Transform _camera;
    private GameObject _body;
    private GameObject _mouth;

    public ParticleSystem chickenEatParticles;
    public ParticleSystem dogDieParticles;

    public float speed = 1f;
    public bool isSpeedLocked;
    private const float SmoothTime = 0.1f;
    private float _smoothVelocity;
    private bool _isDead;

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
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.isGameOver && !_isDead) Died();
        else
        {
            RunSpeed();
        }

        MovePlayerRelativeToCamera2();
    }

    private void Died()
    {
        _isDead = true;
        _body.transform.position = new Vector3(_body.transform.position.x, _body.transform.position.y,
            _body.transform.position.z);
        _mouth.transform.position = new Vector3(_mouth.transform.position.x, 0, _mouth.transform.position.z);
        _body.transform.Rotate(0, 0, 90, Space.Self);
        _mouth.transform.Rotate(0, 0, 0, Space.Self);
        dogDieParticles.Play();
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
            Debug.Log("Running");
        }
        else
        {
            speed = 1;
            Debug.Log("Stopped");
        }
    }

    private void MovePlayerRelativeToCamera()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

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
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var direction = new Vector3(horizontal, 0, vertical).normalized;

        // if (direction.y < 0) direction.y = 0f;
        // direction.Normalize();
        //
        // // Changes the height position of the player
        // if (Input.GetButtonDown("Jump")) direction.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
        // direction.y += GravityValue * Time.deltaTime;

        if (direction.magnitude >= 0.1f)
        {
            var targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _smoothVelocity,
                SmoothTime);
            transform.rotation = Quaternion.Euler(0, rotation, 0);

            var moveDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            _controller.Move(moveDirection.normalized * (speed * Time.deltaTime));
        }
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
        }
        else if (hit.gameObject.CompareTag("Finish Line") && !_gameManager.isGameOver)
        {
            _gameManager.ReachedJungle();
        }
    }
}