using UnityEngine;

/// <summary>
/// Fait par Emile Lucas Wilson
/// Ce script déclenche l'animation de l'ouverture de porte finale :
///     - SAM ANNONCE l'ouverture de la porte
///     - L'animation de l'ouverture de porte commence
///     - c'est tout !!!
/// </summary>
public class FinPartieState : IState
{
    //Ref
    private AnomalieReference _anomalieRef;
    
    //Constructeur
    public FinPartieState(AnomalieReference anomalieRef)
    {
        _anomalieRef = anomalieRef;
    }

    public void OnEnter()
    {
        // Debug.Log("[FIN PARTIE] Le joueur a complété les 5 rounds. Phase finale enclenchée.");

        // // 1. SAM ANNONCE l'ouverture de la porte
        // if (_anomalieRef.samAudioSource != null && _anomalieRef.samSonFinPartie != null)
        // {
        //     _anomalieRef.samAudioSource.PlayOneShot(_anomalieRef.samSonFinPartie);
        // }
        // else
        // {
        //     Debug.LogWarning("[FIN PARTIE] Clip audio de SAM ou AudioSource manquant dans AnomalieReference !");
        // }

        // // 2. L'animation de l'ouverture de porte commence
        // // On va chercher l'Animator de la porte finale via tes références
        // if (_anomalieRef.animatorPorteFinale != null)
        // {
        //     // Assure-toi d'avoir un paramètre Trigger nommé "Ouvrir" (ou autre) dans ton Animator Unity
        //     _anomalieRef.animatorPorteFinale.SetTrigger("Ouvrir"); 
        //     Debug.Log("[FIN PARTIE] Trigger d'animation envoyé à la porte finale.");
        // }
        // else
        // {
        //     Debug.LogWarning("[FIN PARTIE] L'Animator de la porte finale est manquant dans AnomalieReference !");
        // }
    }

    public void Tick()
    {
        // C'est tout !!! Pas besoin de logique en boucle ici.
    }

    public void OnExit()
    {
        // Fin de partie, le joueur est libre ou le jeu se ferme/recommence
    }

    public Color GizmoColor()
    {
        return Color.white;
    }
}