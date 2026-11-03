using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Hazard Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 9f;
    [SerializeField] private float knockTime = 0.25f;

    public int Damage => damage;
    public float KnockbackForce => knockback;
    public float KnockTime => knockTime;

    public void SetDamage(int newDamage) => damage = newDamage;

    private void OnTriggerEnter(Collider other)
    {
        ApplyHazard(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyHazard(other);
    }

    private void ApplyHazard(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out Health health))
        {
            // TakeDamage returns false while in i-frames (slide 19)
            bool hitLanded = health.TakeDamage(damage);

            // Knockback only applies when a hit lands
            if (hitLanded && other.TryGetComponent(out PlayerController playerController))
            {
                Vector3 away = other.transform.position - transform.position;
                away.y = 0f;

                // Guard against exact overlapping positions
                if (away.sqrMagnitude < 0.001f)
                {
                    away = -transform.forward;
                    away.y = 0f;
                }

                Vector3 push = away.normalized * knockback;
                playerController.Knockback(push, knockTime);
            }
        }
    }
}
