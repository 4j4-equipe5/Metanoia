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
        Debug.Log("[FIN PARTIE] Le joueur a complété les 5 rounds. Phase finale enclenchée.");

        _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.Fin);
        _anomalieRef.porteAscenseur.SetTrigger("porteOuverte"); // Remplace "Ouvrir" par le nom exact dans ton Animator
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