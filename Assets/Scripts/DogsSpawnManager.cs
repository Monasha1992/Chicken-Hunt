using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogsSpawnManager : MonoBehaviour
{
    public GameObject[] dogsPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var dogPrefab in dogsPrefabs)
        {
            var area = GameObject.Find($"Dog Patrol Area 2");
            var bounds = GetGroundBounds(area);
            // Generate a random point within the bounds
            var randomX = Random.Range(bounds.min.x, bounds.max.x);
            var randomZ = Random.Range(bounds.min.z, bounds.max.z);

            var spawnPos = new Vector3(randomX, dogPrefab.transform.position.y, randomZ);

            Instantiate(dogPrefab, spawnPos, dogPrefab.transform.rotation);
        }
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