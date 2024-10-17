using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private GameManager _gameManager;

    private CharacterController _controller;
    private Transform _camera;

    public float speed = 6f;
    private const float SmoothTime = 0.1f;
    private float _smoothVelocity;

    private Vector3 _playerVelocity;

    // private float _playerSpeed = 2.0f;
    private const float JumpHeight = 10f;
    private const float GravityValue = -9.81f;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _controller = GetComponent<CharacterController>();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        // shift to speed up
        if (Input.GetKey(KeyCode.LeftShift)) speed = 12;
        else speed = 6;

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            if (direction.y < 0) direction.y = 0f;

            // Changes the height position of the player
            if (Input.GetButtonDown("Jump")) direction.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
            direction.y += GravityValue * Time.deltaTime;

            var targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _smoothVelocity,
                SmoothTime);
            transform.rotation = Quaternion.Euler(0, rotation, 0);


            var moveDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;

            // _controller.Move(_playerVelocity * Time.deltaTime);
            _controller.Move(moveDirection.normalized * (speed * Time.deltaTime));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Chicken"))
        {
            _gameManager.CaughtChicken();
        }
    }
}