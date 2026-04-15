using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private Canvas victoryCanvas;
    [SerializeField] private KeyCode victoryTriggerKey = KeyCode.V;
    private bool hasVictory = false;

    void Update()
    {
        // Trigger victory with key press
        if (Input.GetKeyDown(victoryTriggerKey) && !hasVictory)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        hasVictory = true;

        // Disable player input
        ScriptMouvementPerso playerMovement = FindFirstObjectByType<ScriptMouvementPerso>();
        if (playerMovement != null)
        {
            playerMovement.DisableControls();
        }

        if (victoryCanvas != null)
        {
            victoryCanvas.gameObject.SetActive(true);
        }

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Stop the game
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
