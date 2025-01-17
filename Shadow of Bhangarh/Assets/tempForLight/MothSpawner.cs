using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class MothSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign your moth prefab here")]
    public GameObject mothPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Number of moths to spawn at any given time")]
    public int maxMoths = 10;
    [Tooltip("Time interval between spawns (seconds)")]
    public float spawnInterval = 2f;

    [Header("Moth Settings")]
    [Tooltip("How long each moth lives (seconds)")]
    public float mothLifetime = 10f;
    [Tooltip("Moth's random flying speed")]
    public float mothSpeed = 2f;

    private BoxCollider boxCollider;
    private float spawnTimer = 0f;
    private List<GameObject> activeMoths = new List<GameObject>();

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();

        // Make sure the collider is a trigger if we only want it for bounding
        boxCollider.isTrigger = true;
    }

    void Update()
    {
        // Update timer
        spawnTimer += Time.deltaTime;

        // Spawn a new moth if conditions are met
        if (spawnTimer >= spawnInterval && activeMoths.Count < maxMoths)
        {
            SpawnMoth();
            spawnTimer = 0f;
        }

        // Clean up any null references in the list (if moths got destroyed)
        activeMoths.RemoveAll(m => m == null);
    }

    void SpawnMoth()
    {
        // Get a random point within the BoxCollider
        Vector3 spawnPos = GetRandomPointInBox(boxCollider);
        GameObject newMoth = Instantiate(mothPrefab, spawnPos, Quaternion.identity);

        // Optional: Set the MothSpawner's transform as parent to keep scene organized
        newMoth.transform.parent = transform;

        // Pass settings to the moth's controller script
        MothController mothController = newMoth.GetComponent<MothController>();
        if (mothController != null)
        {
            mothController.SetUpMoth(boxCollider, mothSpeed, mothLifetime);
        }

        activeMoths.Add(newMoth);
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
