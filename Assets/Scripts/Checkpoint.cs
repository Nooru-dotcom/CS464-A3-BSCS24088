using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Renderer checkpointRenderer;
    [SerializeField] private Color activatedColor = Color.cyan;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetCheckpoint(transform);
            }

            if (checkpointRenderer != null)
            {
                checkpointRenderer.material.color = activatedColor;
            }
        }
    }
}
