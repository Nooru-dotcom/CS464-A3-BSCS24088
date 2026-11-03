using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float rotationSpeed = 120f;

    private bool collected = false;

    private void Update()
    {
        // Smooth rotation visual
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // Better: Debug.Log before the if (slide 25)
        Debug.Log($"Coin touched by {other.name} with tag '{other.tag}'");

        if (other.CompareTag("Player"))
        {
            collected = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(value);
            }
            Destroy(gameObject);
        }
    }
}
