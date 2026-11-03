using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.Score >= GameManager.Instance.TargetScore)
                {
                    Debug.Log("Goal reached with sufficient score! You win!");
                    GameManager.Instance.WinGame();
                }
                else
                {
                    Debug.Log($"Goal reached, but need {GameManager.Instance.TargetScore - GameManager.Instance.Score} more coins to win!");
                }
            }
        }
    }
}
