using UnityEngine;
using UnityEngine.AI;

public class DogPatrol : MonoBehaviour
{
    private GameObject _player;
    private NavMeshAgent _agent;

    [SerializeField] private LayerMask groundLayer, playerLayer;

    // Patrol
    [SerializeField] private Vector3 destination;
    private bool _isWalkPointSet;

    private GameObject _area;

    private Bounds _bounds;

    // Start is called before the first frame update
    void Start()
    {
        _area = GameObject.Find("Farm Dog Patrol Area");
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Third Person Player");
        _bounds = GetGroundBounds();
    }


    // Update is called once per frame
    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (!_isWalkPointSet)
        {
            SearchForDestination();
        }
        else
        {
            _agent.SetDestination(destination);
        }

        if (Vector3.Distance(transform.position, destination) < 1)
        {
            _isWalkPointSet = false;
        }
    }

    private void SearchForDestination()
    {
        // var z = Random.Range(-walkRangeZ, walkRangeZ);
        // var x = Random.Range(-walkRangeX, walkRangeX);

        var randomPoint = GetRandomPoint();
        // destination = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        destination = new Vector3(randomPoint.x, transform.position.y, randomPoint.z);

        if (Physics.Raycast(destination, Vector3.down, groundLayer))
        {
            _isWalkPointSet = true;
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