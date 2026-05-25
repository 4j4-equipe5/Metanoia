using UnityEngine;
using UnityEngine.Events; // NE PAS OUBLIER : Requis pour utiliser UnityEvent

public class TimerOuverturePorte : MonoBehaviour
{
    [Header("Configuration du Timer")]
    [SerializeField] private float tempsTotalUnite = 120f; // 120 secondes = 2 minutes
    private float tempsRestant;
    private bool timerEstActif = false;

    [Header("Événement à déclencher")]
    // Cette variable va créer une boîte dans l'Inspecteur pour y glisser n'importe quelle fonction
    public UnityEvent ouTempsEcoule;
    public UnityEvent barrerPorte;

    void Start()
    {
        // Optionnel : Si tu veux que le compte à rebours commence dès le début du niveau
        LancerTimer();
        barrerPorte.Invoke();
    }

    void Update()
    {
        if (!timerEstActif) return;

        if (tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;
        }
        else
        {
            // Le temps est écoulé !
            tempsRestant = 0;
            timerEstActif = false;
            DeclencherEvenement();
        }
    }

    /// <summary>
    /// Active le compte à rebours de 2 minutes
    /// </summary>
    public void LancerTimer()
    {
        tempsRestant = tempsTotalUnite;
        timerEstActif = true;
        Debug.Log("[TIMER] Le compte à rebours de 2 minutes est lancé.");
    }

    /// <summary>
    /// Exécute les fonctions reliées à l'événement
    /// </summary>
    private void DeclencherEvenement()
    {
        Debug.Log("[TIMER] Temps écoulé ! Déclenchement de l'action.");
        
        if (ouTempsEcoule != null)
        {
            ouTempsEcoule.Invoke(); // Appelle TOUTES les méthodes configurées dans l'Inspecteur
        }
    }
}