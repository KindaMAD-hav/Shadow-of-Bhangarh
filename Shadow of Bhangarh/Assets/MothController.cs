using UnityEngine;

public class MothController : MonoBehaviour
{
    [Tooltip("Maximum flying speed")]
    public float speed = 2f;

    [Tooltip("How quickly the moth changes direction (higher = more responsive)")]
    public float steeringResponsiveness = 2f;

    [Tooltip("Time between picking new random targets")]
    public float directionChangeInterval = 2f;

    private BoxCollider boundCollider;
    private Vector3 randomTarget;
    private Vector3 currentVelocity = Vector3.zero;

    private float directionTimer = 0f;

    // Called from the spawner to set up the moth
    public void SetUpMoth(BoxCollider bounds, float mothSpeed, float mothLifetime)
    {
        boundCollider = bounds;
        speed = mothSpeed;
        // Optionally use mothLifetime if you want to handle despawning here
        randomTarget = GetRandomPointInBox(boundCollider);
    }

    void Update()
    {
        // Update the direction timer
        directionTimer += Time.deltaTime;
        if (directionTimer >= directionChangeInterval)
        {
            randomTarget = GetRandomPointInBox(boundCollider);
            directionTimer = 0f;
        }

        // 1. Find the desired direction towards the random target
        Vector3 desiredDirection = (randomTarget - transform.position).normalized;

        // 2. Convert that to a desired velocity
        Vector3 desiredVelocity = desiredDirection * speed;

        // 3. Gradually steer your currentVelocity towards the desiredVelocity
        //    The 'steeringResponsiveness' factor controls how quickly the moth
        //    steers to the new direction.
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            desiredVelocity,
            Time.deltaTime * steeringResponsiveness
        );

        // 4. Move the moth according to currentVelocity
        transform.position += currentVelocity * Time.deltaTime;

        // 5. Optionally rotate to face the movement direction
        if (currentVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(currentVelocity),
                Time.deltaTime * steeringResponsiveness
            );
        }
    }

    Vector3 GetRandomPointInBox(BoxCollider col)
    {
        Vector3 center = col.center + col.transform.position;
        Vector3 size = col.size;
        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);
        return new Vector3(x, y, z);
    }
}
