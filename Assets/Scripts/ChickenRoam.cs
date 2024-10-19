using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ChickenRoam : MonoBehaviour
{
    private GameManager _gameManager;

    // Patrol
    [SerializeField] private LayerMask groundLayer, playerLayer;
    private NavMeshAgent _agent;
    private Vector3 _destination;
    private GameObject _area;
    private Bounds _bounds;
    private bool _isWalkPointSet;
    private bool _isEating;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _area = GameObject.Find("Chicken Farm Walkable Area");
        _agent = GetComponent<NavMeshAgent>();
        _bounds = GetGroundBounds();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_gameManager.hasCaughtChicken) Patrol();
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

        // TODO: Play eating animation
        _isEating = true;
        yield return new WaitForSeconds(30);

        // Clear walkPoint after waiting
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