using UnityEngine;

public class PaintballSticker : MonoBehaviour
{
    private bool hasStuck = false;
    private float spawnTime;
    private const float GRACE_PERIOD = 0.05f; // Seconds to ignore collisions

    void Start()
    {
        spawnTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Prevent sticking to self or immediate hand contact
        if (hasStuck || (Time.time - spawnTime) < GRACE_PERIOD) return;

        hasStuck = true;

        // Stop physics movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        // Parent to the object (Wall/Floor) so it stays put
        transform.SetParent(collision.transform);

        // Visual "Splat" - Rotate to face the surface and flatten
        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point;
        transform.rotation = Quaternion.LookRotation(contact.normal);
        
        // Scale it to look like a flat paint circle
        transform.localScale = new Vector3(0.08f, 0.01f, 0.08f);
    }
}