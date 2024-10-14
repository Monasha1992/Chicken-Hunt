using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;

    private readonly Vector3 _offset = new(0, 5, -7);
    // public float smoothSpeed = 0.125f;
    // public Vector3 locationOffset = new(0, 2, -4);
    //
    // public Vector3 rotationOffset = new(7, 0, 0);

    // Start is called before the first frame update
    void FixedUpdate()
    {
        // var desiredPosition = target.position + target.rotation * locationOffset;
        // var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // transform.position = smoothedPosition;

        // var desiredrotation = target.rotation * Quaternion.Euler(rotationOffset);
        // var smoothedrotation = Quaternion.Lerp(transform.rotation, desiredrotation, smoothSpeed);
        // transform.rotation = smoothedrotation;
        transform.position = target.position + _offset;
    }
}