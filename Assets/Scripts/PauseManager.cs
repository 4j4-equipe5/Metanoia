using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Pause();
        }
    }

    void Pause()
    {
        isPaused = true;

        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(true);
        }
        
        // Unlock and show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;

        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(false);
        }
        
        // Lock and hide cursor again for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        // Disable input controls to prevent memory leak
        // ScriptMouvementPerso playerMovement = FindFirstObjectByType<ScriptMouvementPerso>();
        // if (playerMovement != null)
        // {
        //     playerMovement.DisableControls();
        // }

        // Resume time before loading scene
        Time.timeScale = 1f;
        // Load scene with index 0 (main menu)
        SceneManager.LoadScene(0);
    }

    public static bool IsPaused()
    {
        PauseManager manager = FindFirstObjectByType<PauseManager>();
        return manager != null && manager.isPaused;
    }
}
