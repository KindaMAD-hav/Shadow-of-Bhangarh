using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public Camera playerCamera;
    public float shootRange = 100f;
    public int maxBullets = 3;
    public float shootCooldown = 0.5f;
    public float givenDamageOf = 40f;
    public AudioClip shootingSound;
    public AudioSource audioSource;

    private int currentBullets;
    private float lastShootTime;

    void Start()
    {
        currentBullets = maxBullets;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && Time.time > lastShootTime + shootCooldown)
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if(currentBullets > 0)
        {
            lastShootTime = Time.time;
            currentBullets--;

            if(shootingSound != null)
            {
                audioSource.PlayOneShot(shootingSound);
            }
            RaycastHit hit;
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootRange))
            {
                Debug.Log("Hit:" + hit.collider.name);
                //damage enemy
                    }
            Debug.Log("Shot fired. Bullet left" + currentBullets);
        }
        else
        {
            Debug.Log("No bullets Left");
        }
    }
}
