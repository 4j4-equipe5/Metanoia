using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    //ajout fait par Emile
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private Slider sliderX;
    [SerializeField] private Slider sliderY;
     private ScriptMouvementPerso joueur; 
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Pause();
        }
    }
    void Awake()
    {
         joueur = FindFirstObjectByType<ScriptMouvementPerso>(); 
    }
    public void ChangerX(float valeur)
    {
        if(joueur != null) joueur.sensitivityX = valeur;
    }
    public void ChangerY(float valeur)
    {
        if(joueur != null) joueur.sensitivityY = valeur;
    }
    void Pause()
    {
        isPaused = true;


        if(sliderX != null) sliderX.value = joueur.sensitivityX;
        if(sliderY != null) sliderY.value = joueur.sensitivityY;

        sliderX.onValueChanged.AddListener(ChangerX);
        sliderY.onValueChanged.AddListener(ChangerY);
        // Disable player input so UI can be interacted with
        ScriptMouvementPerso playerMovement = FindFirstObjectByType<ScriptMouvementPerso>();
        if (playerMovement != null)
        {
            playerMovement.DisableControls();
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(true);
        }
        
        // Unlock and show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Time.timeScale = 0f;
        musicAudioSource.Pause();
    }

    public void Resume()
    {
        isPaused = false;

        // Re-enable player input
        ScriptMouvementPerso playerMovement = FindFirstObjectByType<ScriptMouvementPerso>();
        if (playerMovement != null)
        {
            playerMovement.EnableControls();
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(false);
        }
        
        // Lock and hide cursor again for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Time.timeScale = 1f;
        musicAudioSource.UnPause();
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
