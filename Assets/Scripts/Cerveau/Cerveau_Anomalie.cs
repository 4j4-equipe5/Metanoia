using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Fait par Emile Lucas Wilson
/// 
/// Ce script permet de différencier les états du systèmes d'anomalies et personnaliser le state machine
///     - Anomalie spawn
///     - normal enlevé
///     - Gestion des rounds
///     - Gestion des boutons d
///     - Gestion de la fin
/// </summary>
public class Cerveau_Anomalie : MonoBehaviour
{
    // Ref
    private StateMachine stateMachine;
    private AnomalieReference anomalieRef;

    void Awake()
    {
        // 1. Récupère les components du système
        anomalieRef = GetComponent<AnomalieReference>();

        // 2. initialisation du FSM
        stateMachine = new StateMachine();

        //3. Transition Unique
        var genererRound = new GenererRound(anomalieRef);
        var attenteJoueur = new AttenteJoueurState(anomalieRef);
        var resolution = new ResolutionState(anomalieRef);
        var finPartie = new FinPartieState(anomalieRef);

        //4. Définition des transitions
        // elle permet d'utiliser At et AtAny pour ajouter des transitions entre les états
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition); // Fonction pour ajouter une transition
        void AtAny(IState to, Func<bool> condition) => stateMachine.AddAnyTransition (to, condition); // Fonction pour ajouter une transition qui peut se produire à n'importe quel moment

        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        // Configuration des Transitions
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        // ÉTAPE A : Une fois que le round est généré et les objets sont placés, on passe en attente des décisions du Joueur
        At(genererRound, attenteJoueur, () => genererRound.isComplete);
        // ÉTAPE B : Une fois que le joueur décide, on résolue sa décision
        At(attenteJoueur, resolution, () => attenteJoueur.decisionFait);
        // ÉTAPE C : Après la résolution on retourne a générer un nouveauRound
        At(resolution, genererRound, () => resolution.isComplete && anomalieRef.roundsActuel < 6);
        // Étape D : Après la 5ieme résolution on finie le jeu
        At(resolution, finPartie, () => resolution.isComplete && anomalieRef.roundsActuel >= 6 );

        stateMachine.SetState(genererRound);
    }

    void Update()
    {
        stateMachine.Tick();
    }
    private void OnDrawGizmos()
    {
        if (stateMachine == null) return;
        
        Gizmos.color = stateMachine.GetGizmoColor();
        Gizmos.DrawSphere(this.transform.position + Vector3.up * 10f, 2.5f);
    }
}