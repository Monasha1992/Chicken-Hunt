using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private readonly float _speed = 6.0f;
    private readonly float _turnSpeed = 25.0f;
    private float _forwardInput;
    private float _horizontalInput;

    // Update is called once per frame
    private void Update()
    {
        // Get player inputs
        _horizontalInput = Input.GetAxis("Horizontal");
        _forwardInput = Input.GetAxis("Vertical");

        // Move the vehicle forward
        // transform.Translate(0, 0, 0.01f);
        transform.Translate(Vector3.forward * (Time.deltaTime * _speed * _forwardInput));
        // transform.Translate(Vector3.right * (Time.deltaTime * turnSpeed * horizontalInput));
        // Rotate the vehicle according to horizontal input
        transform.Rotate(Vector3.up * (Time.deltaTime * _turnSpeed * _horizontalInput));
    }
}