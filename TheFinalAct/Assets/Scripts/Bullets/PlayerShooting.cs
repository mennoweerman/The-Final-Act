using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 0.3f;

    private float nextFireTime;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime) // Linker muisknop ingedrukt
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null)
        {
            Debug.LogWarning("❌ BulletPrefab of ShootPoint niet ingesteld!");
            return;
        }

        // ✅ Richting = camera forward
        Transform cam = Camera.main.transform;
        Vector3 direction = cam.forward;

        // Spawn de bullet in camera-richting
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(direction));

        // Extra: als je meteen velocity wilt zetten via Rigidbody (handig bij physics)
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * rb.linearVelocity.magnitude; 
        }
    }
}
