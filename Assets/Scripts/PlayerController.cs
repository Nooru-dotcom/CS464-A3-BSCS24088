using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private LayerMask groundLayers = ~0;

    public float speedMultiplier = 1f;

    private Rigidbody rb;
    private float noControlUntil;
    private Coroutine boostCoroutine;
    private bool isGrounded;

    public float Speed => speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure rotation is locked along X and Z axes
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Ground check using a small downward sphere / ray
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.35f, groundLayers, QueryTriggerInteraction.Ignore);

        // Jump input
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(v.x, jumpForce, v.z);
        }
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        move = Vector3.ClampMagnitude(move, 1f) * speed * speedMultiplier;

        // Steering pauses during knockback window
        if (Time.time >= noControlUntil)
        {
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }
    }

    /// <summary>
    /// Knocks back the player, zeroing horizontal speed and pausing steering for knockTime seconds.
    /// Uses ForceMode.Impulse.
    /// </summary>
    public void Knockback(Vector3 push, float seconds)
    {
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0f, v.y, 0f);
        rb.AddForce(push, ForceMode.Impulse);
        noControlUntil = Time.time + seconds;
    }

    /// <summary>
    /// Boosts player speed for duration seconds.
    /// Follows the "Refresh" rule: cancels any running boost and starts a fresh one.
    /// </summary>
    public void BoostSpeed(float multiplier, float duration)
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }
        boostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        boostCoroutine = null;
    }

    public void ResetMovement()
    {
        noControlUntil = 0f;
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }
        speedMultiplier = 1f;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
