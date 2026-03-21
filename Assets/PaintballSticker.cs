using UnityEngine;

public class PaintballSticker : MonoBehaviour
{
    private bool hasStuck = false;
    private float spawnTime;
    private const float GRACE_PERIOD = 0.05f;
    public float damageValue = 10.0f; // How much damage one paintball does

    void Start()
    {
        spawnTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasStuck || (Time.time - spawnTime) < GRACE_PERIOD) return;

        // 1. Check if we hit a Target
        TargetSlime target = collision.gameObject.GetComponentInParent<TargetSlime>();
    if (target != null)
    {
        target.Got(damageValue);
        Destroy(gameObject); 
        return;
    }

        // 2. Otherwise, stick to the wall/floor (MRUK)
        hasStuck = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        transform.SetParent(collision.transform);

        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point;
        transform.rotation = Quaternion.LookRotation(contact.normal);
        transform.localScale = new Vector3(0.08f, 0.01f, 0.08f);
        
        Destroy(gameObject, 10f);
    }
}