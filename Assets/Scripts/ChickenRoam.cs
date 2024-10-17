using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ChickenRoam : MonoBehaviour
{
    private GameObject _player;
    private GameObject _foxMouth;
    private BoxCollider _collider;
    private Rigidbody _rigidbody;

    [SerializeField] private bool hasCaught;

    // Patrol
    [SerializeField] private LayerMask groundLayer, playerLayer;
    private NavMeshAgent _agent;
    private Vector3 _destination;
    private GameObject _area;
    private Bounds _bounds;
    private bool _isWalkPointSet;
    private bool _isEating;

    public Vector3 mouth;

    // Start is called before the first frame update
    void Start()
    {
        _area = GameObject.Find("Chicken Farm Walkable Area");
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _player = GameObject.Find("Third Person Player");
        _foxMouth = GameObject.Find("Mouth");
        mouth = _foxMouth.transform.position;
        _bounds = GetGroundBounds();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!hasCaught) Patrol();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !hasCaught)
        {
            hasCaught = true;

            _agent.enabled = false;
            _collider.enabled = false;
            _rigidbody.useGravity = false;

            var position = _foxMouth.transform.position;
            // position.y = _foxMouth.transform.position.y;
            transform.SetParent(_player.transform);
            transform.SetPositionAndRotation(position, _foxMouth.transform.rotation);
        }
    }

    private void Patrol()
    {
        if (!_isWalkPointSet)
        {
            SearchForDestination();
        }
        else
        {
            _agent.SetDestination(_destination);
        }

        if (Vector3.Distance(transform.position, _destination) < 1)
        {
            StartCoroutine(Eating());
        }
    }

    private IEnumerator Eating()
    {
        if (_isEating) yield break;

        Debug.Log($"isWalkPointSet -{_isWalkPointSet} - Stared eating");
        // TODO: Play eating animation
        _isEating = true;
        yield return new WaitForSeconds(30);

        // Clear walkPoint after waiting
        Debug.Log($"isWalkPointSet -{_isWalkPointSet} - Finished eating");
        // TODO: Play idle animation
        _isWalkPointSet = false;
        _isEating = false;
    }

    private void SearchForDestination()
    {
        var randomPoint = GetRandomPoint();
        _destination = new Vector3(randomPoint.x, 0, randomPoint.z);

        if (Physics.Raycast(_destination, Vector3.down, groundLayer))
        {
            _isWalkPointSet = true;
            // TODO: Play walking animation
        }
    }

    private Vector3 GetRandomPoint()
    {
        // Generate a random point within the bounds
        var randomX = Random.Range(_bounds.min.x, _bounds.max.x);
        // var randomY = Random.Range(_bounds.min.y, _bounds.max.y);
        var randomZ = Random.Range(_bounds.min.z, _bounds.max.z);

        return new Vector3(randomX, transform.position.y, randomZ);
    }

    private Bounds GetGroundBounds()
    {
        // Get the bounds of the GameObject
        if (_area.TryGetComponent<Collider>(out var childCollider))
        {
            return childCollider.bounds;
        }

        if (_area.TryGetComponent<Renderer>(out var childRenderer))
        {
            return childRenderer.bounds;
        }

        return new Bounds();
    }
}