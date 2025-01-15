using UnityEngine;

public class MothController : MonoBehaviour
{
    private BoxCollider boundCollider;
    private float speed;
    private float lifetime;

    private float timeAlive = 0f;

    // We’ll track a “target” random point within the bounds
    private Vector3 randomTarget;

    // How often the moth picks a new random direction
    [Tooltip("How often (in seconds) the moth changes flight direction")]
    public float directionChangeInterval = 2f;

    private float directionTimer = 0f;

    /// <summary>
    /// Called from the spawner to set up the moth’s parameters.
    /// </summary>
    public void SetUpMoth(BoxCollider bounds, float mothSpeed, float mothLifetime)
    {
        boundCollider = bounds;
        speed = mothSpeed;
        lifetime = mothLifetime;

        // Pick an initial random target
        randomTarget = GetRandomPointInBox(boundCollider);
    }

    void Update()
    {
        // Track how long this moth has been alive
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);  // Despawn
            return;
        }

        // Update the direction timer
        directionTimer += Time.deltaTime;
        if (directionTimer >= directionChangeInterval)
        {
            randomTarget = GetRandomPointInBox(boundCollider);
            directionTimer = 0f;
        }

        // Move towards the random target
        Vector3 direction = (randomTarget - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Optionally, rotate to face the direction of movement
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// Returns a random position within the box collider’s bounds.
    /// </summary>
    Vector3 GetRandomPointInBox(BoxCollider col)
    {
        // The center is relative to the transform
        Vector3 center = col.center + col.transform.position;
        Vector3 size = col.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        return new Vector3(x, y, z);
    }
}
