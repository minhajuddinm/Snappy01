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

        int targetLayer = LayerMask.NameToLayer("Target");
        if (targetLayer != -1) gameObject.layer = targetLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Paintball")) return;

        PaintballSticker sticker = other.GetComponent<PaintballSticker>();

        float damage = 1.0f;
        if (sticker != null)
        {
            damage = sticker.damageValue;
        }

        Got(damage);
        Destroy(other.gameObject);
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

        if (GameSystem.Instance != null)
            GameSystem.Instance.TargetDestroyed(pointValue);

        gameObject.SetActive(false);
    }
}