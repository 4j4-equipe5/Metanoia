
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Paramètres de victoire")]
    [SerializeField] private int monstreMort = 0; // nombre de monstres tués, peut être utilisé pour déclencher la victoire
    [SerializeField] private int victoireCondition = 3; // nombre de monstres à tuer pour déclencher la victoire
    [SerializeField] private Canvas victoryCanvas;
    [SerializeField] private KeyCode victoryTriggerKey = KeyCode.V;
    [SerializeField] public AudioSource musicAudioSource;
    [SerializeField] public GameObject pauseMenu; // référence au menu de pause pour le désactiver lors de la victoire

    private bool hasVictory = false;
    public void AjouterMort() // ceci est seulement utiliser pour la démo, dans une version finale on pourrait vouloir faire un système de score plus complexe ou déclencher la victoire d'une autre manière
    {
        monstreMort++;
        Debug.Log("Monstre tué ! Total : " + monstreMort);
        if (monstreMort >= victoireCondition && !hasVictory)
        {
            TriggerVictory();
        }

    }
    void Update()
    {
        // Trigger victory with key press
        if (Input.GetKeyDown(victoryTriggerKey) && !hasVictory)
        {
            TriggerVictory();
        }
    }

    public void TriggerVictory()
    {
        hasVictory = true;

        // Disable player input
        ScriptMouvementPerso playerMovement = FindFirstObjectByType<ScriptMouvementPerso>();
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false); // désactive le menu de pause si il est actif
            PauseManager pauseManager = pauseMenu.GetComponent<PauseManager>();
            if (pauseManager != null)
            {
                pauseManager.enabled = false; // désactive le script de pause pour éviter les conflits
            }
        }
        if (playerMovement != null)
        {
            playerMovement.DisableMovement(); // désactive complètement le script de mouvement
        }

        victoryCanvas?.gameObject.SetActive(true);
        musicAudioSource?.Stop(); // Arrête la musique si elle est trouvable
        
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

