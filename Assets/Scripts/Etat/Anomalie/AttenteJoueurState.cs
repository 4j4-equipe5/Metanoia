using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Fait par Emile Lucas Wilson 
/// Cette état attends le choix que joueur fait en pesant sur les boutons:
///     - la télé contient un écran qui permet de le joueur de sélectionner le nb d'anomalie de 1 à 10
///     - après sa décision ces la résolution
/// </summary>
public class AttenteJoueurState : IState
{
    //Ref
    private AnomalieReference _anomalieRef;
    public bool decisionFait {get; private set;}

    // constructeur

    public AttenteJoueurState(AnomalieReference anomalieRef)
    {
        _anomalieRef = anomalieRef;
    }

    public void OnEnter()
    {
        decisionFait = false;
    }
    public void Tick()
    {
        // si le joueur pèse sur le bouton enter
        // int la réponse du joueur
        if (_anomalieRef.scriptTeleUI.choixFait)
        {

            _anomalieRef.reponseJoueur = _anomalieRef.scriptTeleUI.numeroPiece;
            decisionFait = true;
        }
    }
    public void OnExit(){}
    public Color GizmoColor()
    {
        return Color.black;
    }
}