using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Flight Settings")]
    [Tooltip("Maximum flying speed")]
    public float speed = 5f;

    [Tooltip("How quickly the bat changes direction (higher = more responsive)")]
    public float steeringResponsiveness = 3f;

    [Tooltip("Time between picking new random targets")]
    public float directionChangeInterval = 1.5f;

    [Header("Swoop Motion")]
    [Tooltip("Frequency of the sinusoidal swoop")]
    public float swoopFrequency = 2f;

    [Tooltip("Amplitude of the swoop (vertical movement)")]
    public float swoopAmplitude = 0.5f;

    private BoxCollider boundCollider;
    private Vector3 randomTarget;
    private Vector3 currentVelocity = Vector3.zero;

    private float directionTimer = 0f;
    private float baseHeight; // Store initial spawn height to apply swoop offset

    // Called from the spawner to set up the bat
    public void SetUpBat(BoxCollider bounds, float batSpeed, float batLifetime)
    {
        boundCollider = bounds;
        speed = batSpeed;
        // Optionally use batLifetime if you want to handle despawning here

        randomTarget = GetRandomPointInBox(boundCollider);
        // Store the initial y-position to anchor our sinusoidal swoop
        baseHeight = transform.position.y;
    }

    void Update()
    {
        // Update the direction timer
        directionTimer += Time.deltaTime;
        if (directionTimer >= directionChangeInterval)
        {
            // Pick a new random target more frequently
            randomTarget = GetRandomPointInBox(boundCollider);
            directionTimer = 0f;
        }

        // 1. Find the desired direction towards the random target
        Vector3 desiredDirection = (randomTarget - transform.position).normalized;

        // 2. Convert that to a desired velocity
        Vector3 desiredVelocity = desiredDirection * speed;

        // 3. Gradually steer currentVelocity towards the desiredVelocity
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            desiredVelocity,
            Time.deltaTime * steeringResponsiveness
        );

        // 4. Compute a sinusoidal vertical offset to simulate swooping
        float swoopOffset = Mathf.Sin(Time.time * swoopFrequency) * swoopAmplitude;

        // 5. Construct final movement vector
        Vector3 movement = currentVelocity * Time.deltaTime;

        // Apply the sinusoidal offset to the Y-axis
        movement.y += swoopOffset * Time.deltaTime;
        // Note: We multiply by Time.deltaTime again so that the swoop effect isn't too fast.
        // You can remove that if you prefer a faster vertical movement.

        // 6. Move the bat according to movement
        transform.position += movement;

        // 7. Optionally rotate to face the movement direction
        //    Keep in mind the swoop might cause the Y portion to fluctuate
        //    so we can zero out the y-component if you don't want the bat
        //    to pitch up/down drastically.
        Vector3 flatVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        if (flatVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(flatVelocity),
                Time.deltaTime * steeringResponsiveness
            );
        }

        // Optional: clamp position within bounding box if you don’t want them escaping
        ClampPositionInBox();
    }

    // Restrict the bat to remain within the BoxCollider
    private void ClampPositionInBox()
    {
        if (!boundCollider) return; // Safety check

        // Get collider world min/max
        Vector3 min = boundCollider.bounds.min;
        Vector3 max = boundCollider.bounds.max;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        pos.z = Mathf.Clamp(pos.z, min.z, max.z);
        transform.position = pos;
    }

    // Returns a random position within the box collider’s bounds.
    private Vector3 GetRandomPointInBox(BoxCollider col)
    {
        if (col == null)
        {
            // Fallback: if there's no collider, just pick something near origin
            return transform.position + Random.insideUnitSphere * 2f;
        }

        Vector3 center = col.center + col.transform.position;
        Vector3 size = col.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);
        return new Vector3(x, y, z);
    }
}
