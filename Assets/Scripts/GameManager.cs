using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Progression")]
    [SerializeField] private int targetScore = 5;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private Transform respawnPoint;

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button winRestartButton;
    [SerializeField] private Button loseRestartButton;

    private int currentScore = 0;
    private int currentLives;
    private bool isGameOver = false;
    private GameObject playerObject;

    public int Score => currentScore;
    public int TargetScore => targetScore;
    public int Lives => currentLives;
    public Transform RespawnPoint => respawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;
        currentLives = startingLives;
    }

    private void Start()
    {
        // Find player in scene if not cached
        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            playerObject = player.gameObject;
        }

        // Hook up restart buttons
        if (winRestartButton != null)
        {
            winRestartButton.onClick.AddListener(RestartLevel);
        }
        if (loseRestartButton != null)
        {
            loseRestartButton.onClick.AddListener(RestartLevel);
        }

        // Hide end panels at start
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        UpdateScoreHUD();
        UpdateLivesHUD();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        currentScore += amount;
        UpdateScoreHUD();

        // Check if level auto-wins upon collecting target score
        if (currentScore >= targetScore && winPanel != null && !winPanel.activeSelf)
        {
            // Win condition can trigger directly or via Goal trigger.
            // When targetScore reached, level is ready to win or auto-wins.
        }
    }

    public void UpdateScoreHUD()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore} / {targetScore}";
        }
    }

    public void UpdateHealthHUD(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = $"Hearts: {Mathf.Max(current, 0)} / {max}";
        }
    }

    public void UpdateLivesHUD()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {currentLives}";
        }
    }

    public void SetCheckpoint(Transform point)
    {
        respawnPoint = point;
        Debug.Log($"Checkpoint set to: {point.position}");
    }

    public void OnPlayerFall()
    {
        if (isGameOver) return;

        Debug.Log("Player fell into kill zone! Lost a life.");
        OnPlayerDied();
    }

    public void OnPlayerDied()
    {
        if (isGameOver) return;

        currentLives--;
        UpdateLivesHUD();

        if (currentLives <= 0)
        {
            ShowGameOver();
        }
        else
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        // Wait a beat before respawn (slide 30)
        yield return new WaitForSeconds(0.6f);

        if (playerObject == null)
        {
            PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
            if (pc != null) playerObject = pc.gameObject;
        }

        if (playerObject != null)
        {
            Vector3 spawnPos = respawnPoint != null ? respawnPoint.position + Vector3.up * 1.0f : Vector3.up;
            playerObject.transform.position = spawnPos;

            if (playerObject.TryGetComponent(out PlayerController controller))
            {
                controller.ResetMovement();
            }

            if (playerObject.TryGetComponent(out Health health))
            {
                // Respawn with protection (i-frames)
                health.RespawnReset(1.5f);
            }
        }
    }

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Freeze time upon win
        Time.timeScale = 0f;
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // Freeze time upon game over
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Restarts the level: time restored to 1 first, then reloads the active scene.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
