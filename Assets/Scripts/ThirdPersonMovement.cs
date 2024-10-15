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

            var moveDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            _controller.Move(moveDirection.normalized * (speed * Time.deltaTime));
        }
    }
}