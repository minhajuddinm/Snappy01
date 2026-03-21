using UnityEngine;

public class TargetSlime : MonoBehaviour
{
    [Header("Stats")]
    public float health = 5.0f;
    public int pointValue = 10;

    [Header("Effects")]
    public ParticleSystem DestroyedEffect;
    
    private bool m_Destroyed = false;
    private float m_CurrentHealth;

    void Awake()
    {
        m_CurrentHealth = health;
        
        // Ensure the target is on the correct layer
        int targetLayer = LayerMask.NameToLayer("Target");
        if (targetLayer != -1) gameObject.layer = targetLayer;
    }

    // Backup: If the ball is too fast for a collision, a Trigger will often catch it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paintball")) 
        {
            Got(1.0f);
            Destroy(other.gameObject); // Clean up the paintball
        }
    }

    public void Got(float damage)
    {
        if (m_Destroyed) return;

        m_CurrentHealth -= damage;
        Debug.Log("Target hit! Health: " + m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        m_Destroyed = true;

        if (DestroyedEffect != null)
        {
            Instantiate(DestroyedEffect, transform.position, Quaternion.identity);
        }

        // Notify GameSystem if it exists
        if (GameSystem.Instance != null)
            GameSystem.Instance.TargetDestroyed(pointValue);

        gameObject.SetActive(false);
    }
}