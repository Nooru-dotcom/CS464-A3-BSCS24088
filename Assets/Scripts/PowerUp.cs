using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [SerializeField] private float speedMultiplier = 1.6f;
    [SerializeField] private float duration = 5.0f;
    [SerializeField] private float rotateSpeed = 100f;

    private bool collected = false;

    private void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            collected = true;

            // Timer outlives the pickup: player runs the timer coroutine
            // Rule: Refresh (stops current boost coroutine, starts fresh duration)
            player.BoostSpeed(speedMultiplier, duration);

            Debug.Log($"Speed Power-Up collected! Duration: {duration}s, Multiplier: {speedMultiplier}x");
            Destroy(gameObject);
        }
    }
}
