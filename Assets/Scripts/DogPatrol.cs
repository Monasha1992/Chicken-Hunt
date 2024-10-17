using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class DogPatrol : MonoBehaviour
{
    // private GameObject _player;
    private NavMeshAgent _agent;

    [SerializeField] private LayerMask groundLayer, playerLayer;

    // Patrol
    private Vector3 _destination;
    private bool _isWalkPointSet;

    private readonly GameObject[] _areas = new GameObject[3];
    private int _lastAreaIndex = 2;

    private readonly Bounds[] _bounds = new Bounds[3];

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < _areas.Length; i++)
        {
            var area = GameObject.Find($"Dog Patrol Area {i + 1}");
            _areas[i] = (area);
            _bounds[i] = GetGroundBounds(area);
        }

        _agent = GetComponent<NavMeshAgent>();
        // _player = GameObject.Find("Third Person Player");
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
            _agent.SetDestination(_destination);
        }

        if (Vector3.Distance(transform.position, _destination) < 5)
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
        _destination = new Vector3(randomPoint.x, 0, randomPoint.z);

        if (Physics.Raycast(_destination, Vector3.down, groundLayer))
        {
            _isWalkPointSet = true;
        }
    }

    private Vector3 GetRandomPoint()
    {
        Bounds bounds;
        // get the next bound according to the current bound
        switch (_lastAreaIndex)
        {
            case 0:
            {
                int[] accessibleAreasIndexes = { 0, 1 };
                var randomIndex = Random.Range(0, 2);
                var index = accessibleAreasIndexes[randomIndex];
                bounds = _bounds[index];
                _lastAreaIndex = index;
                break;
            }
            case 1:
            {
                int[] accessibleAreasIndexes = { 0, 1, 2 };
                var randomIndex = Random.Range(0, 3);
                var index = accessibleAreasIndexes[randomIndex];
                bounds = _bounds[index];
                _lastAreaIndex = index;
                break;
            }
            case 2:
            {
                int[] accessibleAreasIndexes = { 1, 2 };
                var randomIndex = Random.Range(0, 2);
                var index = accessibleAreasIndexes[randomIndex];
                bounds = _bounds[index];
                _lastAreaIndex = index;
                break;
            }
            default:
            {
                bounds = _bounds.Last();
                _lastAreaIndex = 2;
                break;
            }
        }

        // Generate a random point within the bounds
        var randomX = Random.Range(bounds.min.x, bounds.max.x);
        // var randomY = Random.Range(_bounds.min.y, _bounds.max.y);
        var randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, transform.position.y, randomZ);
    }

    private Bounds GetGroundBounds(GameObject area)
    {
        // Get the bounds of the GameObject
        if (area.TryGetComponent<Collider>(out var childCollider))
        {
            return childCollider.bounds;
        }

        if (area.TryGetComponent<Renderer>(out var childRenderer))
        {
            return childRenderer.bounds;
        }

        return new Bounds();
    }
}