using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class DogPatrol : MonoBehaviour
{
    private GameManager _gameManager;

    private GameObject _player;
    private NavMeshAgent _agent;

    [SerializeField] private LayerMask groundLayer;

    // Patrol
    private Vector3 _destination;
    private bool _isWalkPointSet;

    private readonly GameObject[] _areas = new GameObject[3];
    public int lastAreaIndex = 2;

    private readonly Bounds[] _bounds = new Bounds[3];

    private FieldOfView _fov;
    private bool _isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _agent = GetComponent<NavMeshAgent>();
        _fov = GetComponent<FieldOfView>();
        _player = GameObject.Find("Third Person Player");

        _agent.speed = 2;

        for (var i = 0; i < _areas.Length; i++)
        {
            var area = GameObject.Find($"Dog Patrol Area {i + 1}");
            _areas[i] = area;
            _bounds[i] = GetGroundBounds(area);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if ((_fov.canSeePlayer || _gameManager.isAttacking) && !_gameManager.isGameOver) ChasePlayer();
        else Patrol();
    }

    private void ChasePlayer()
    {
        _agent.speed = 3;
        _agent.SetDestination(_player.transform.position);
    }

    private void Patrol()
    {
        _agent.speed = 1;

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
        switch (lastAreaIndex)
        {
            case 0:
            {
                int[] accessibleAreasIndexes = { 0, 1 };
                var randomIndex = Random.Range(0, 2);
                var index = accessibleAreasIndexes[randomIndex];
                bounds = _bounds[index];
                lastAreaIndex = index;
                break;
            }
            case 1:
            {
                int[] accessibleAreasIndexes = { 0, 1, 2 };
                var randomIndex = Random.Range(0, 3);
                var index = accessibleAreasIndexes[randomIndex];
                bounds = _bounds[index];
                lastAreaIndex = index;
                break;
            }
            case 2:
            {
                int[] accessibleAreasIndexes = { 1, 2 };
                var randomIndex = Random.Range(0, 2);
                var index = accessibleAreasIndexes[randomIndex];
                bounds = _bounds[index];
                lastAreaIndex = index;
                break;
            }
            default:
            {
                bounds = _bounds.Last();
                lastAreaIndex = 2;
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


    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !_isAttacking)
        {
            _isAttacking = true;
            _gameManager.GotAttacked();

            StartCoroutine(_gameManager.Delay(0.1f));
            _isAttacking = false;
        }
    }
}