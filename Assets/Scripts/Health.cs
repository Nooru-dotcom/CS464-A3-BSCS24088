using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float iFrames = 1.0f;

    private int currentHealth;
    private float invulnerableUntil;
    private Renderer[] renderers;
    private bool isDead;

    public int CurrentHealth => Mathf.Max(currentHealth, 0);
    public int MaxHealth => maxHealth;
    public float IFrames => iFrames;

    private void Awake()
    {
        currentHealth = maxHealth;
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthHUD(CurrentHealth, maxHealth);
        }
    }

    /// <summary>
    /// Attempts to apply damage. Returns true if hit lands; returns false if player is in i-frames.
    /// </summary>
    public bool TakeDamage(int damage)
    {
        if (Time.time < invulnerableUntil || isDead)
        {
            return false;
        }

        currentHealth -= damage;
        invulnerableUntil = Time.time + iFrames;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthHUD(CurrentHealth, maxHealth);
        }

        StartCoroutine(DamageFlashRoutine());

        // Down at <= 0, never == 0 (slide 29)
        if (currentHealth <= 0)
        {
            Die();
        }

        return true;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        float flashEndTime = Time.time + iFrames;
        bool visible = true;

        while (Time.time < flashEndTime)
        {
            visible = !visible;
            SetRenderersVisible(visible);
            yield return new WaitForSeconds(0.1f);
        }

        SetRenderersVisible(true);
    }

    private void SetRenderersVisible(bool visible)
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = visible;
            }
        }
    }

    public void RespawnReset(float protectionDuration = 1.5f)
    {
        isDead = false;
        currentHealth = maxHealth;
        invulnerableUntil = Time.time + protectionDuration;
        SetRenderersVisible(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthHUD(CurrentHealth, maxHealth);
        }

        StartCoroutine(DamageFlashRoutine());
    }
}
