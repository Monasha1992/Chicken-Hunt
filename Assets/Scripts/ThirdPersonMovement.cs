using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController _controller;
    private Transform _camera;

    public float speed = 6f;
    private readonly float _smoothTime = 0.1f;
    private float _smoothVelocity;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;


    private void Start()
    {
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        
        var direction = new Vector3(horizontal, 0, vertical).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            var targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _smoothVelocity,
                _smoothTime);
            transform.rotation = Quaternion.Euler(0, rotation, 0);
            // var newRotation = Quaternion.Euler(0, rotation, 0);
        
            var moveDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            _controller.Move(moveDirection.normalized * (speed * Time.deltaTime));
        }

        // groundedPlayer = _controller.isGrounded;
        // if (groundedPlayer && playerVelocity.y < 0)
        // {
        //     playerVelocity.y = 0f;
        // }
        //
        // var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // _controller.Move(move * (Time.deltaTime * playerSpeed));
        //
        // if (move != Vector3.zero)
        // {
        //     gameObject.transform.forward = move;
        // }
        //
        // // Changes the height position of the player
        // if (Input.GetButtonDown("Jump") && groundedPlayer)
        // {
        //     playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        // }
        //
        // playerVelocity.y += gravityValue * Time.deltaTime;
        //
        // // var targetRotation = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        // // var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _smoothVelocity,
        // //     _smoothTime);
        // // transform.rotation = Quaternion.Euler(0, rotation, 0);
        // _controller.Move(playerVelocity * Time.deltaTime);
    }
}