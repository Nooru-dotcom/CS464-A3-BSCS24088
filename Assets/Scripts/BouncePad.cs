using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceVelocity = 12f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Rigidbody rb))
        {
            // Zero out vertical velocity so the bounce height is completely consistent
            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(v.x, 0f, v.z);

            // VelocityChange: one kick, mass ignored (Part A4 requirement)
            rb.AddForce(Vector3.up * bounceVelocity, ForceMode.VelocityChange);

            Debug.Log($"BouncePad launched player with {bounceVelocity} m/s using ForceMode.VelocityChange");
        }
    }
}
