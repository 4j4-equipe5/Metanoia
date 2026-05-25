using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Fait par Emile Lucas Wilson
/// Etat qui déclanche la mort d'un ennemie 
///     - Ragdoll
///     - Ajout de points
///
/// </summary>
public class DeathState : IState
{
    // Référence 
    private EnnemyReferences _ennemyRef;
    private Transform player;
    float timePassed;
    Vector3 startPosition;

    // Constructeur
    public DeathState (EnnemyReferences ennemyRef)
    {
        _ennemyRef = ennemyRef;
        player = ennemyRef.player;
    }
    public void OnEnter()
    {
        _ennemyRef.agent.isStopped = true; // Arrête le NavMeshAgent pour le Stun
        _ennemyRef.agent.enabled = false; // Désactive le NavMeshAgent pour permettre un contrôle total de l'ennemi pendant le Stun
        _ennemyRef.animEnnemi.enabled = false; // Désactive l'Animator pour le Stun
        _ennemyRef.forceFreezeHips.enabled = false; // Désactive le script de freeze des hanches pour permettre au ragdoll de réagir correctement
        Debug.Log("Ennemi est stunned");

        startPosition = _ennemyRef.transform.position;
        // On veut récupérer la Direction pour le ragdoll suit la direction du joueur
        Vector3 direction = (startPosition - player.position).normalized;

        // normaliser la direction en y
        direction.y = 0;
        direction.Normalize();

        // Ajout du recul pour projeter l'ennemi vers le haut
        direction.y = 0.7f; // Placeholder, à ajuster selon les besoins pour la composante verticale du recul
        direction.Normalize();
        // On applique une force de ragdoll dans la direction du joueur, avec une magnitude dépendant de l'arme utilisée
        // Placeholder, à ajuster selon les besoins pour la force du ragdoll
        Vector3 force = direction * _ennemyRef.forceDeRagdoll; // + une composante verticale pour faire lever l'ennemi un peu dans les airs


        // TODO: Ajout des Points
        _ennemyRef.sonManager.SonMiann(SonManager.IdSonMiann.Mort);
        timePassed = 0f;
    }

    public void Tick()
    {
        timePassed += Time.deltaTime;

        if (timePassed >= 5f && !_ennemyRef.isDead)
        {
            _ennemyRef.isDead = true;
        }
    }
    public void OnExit()
    {
    }
    public Color GizmoColor()
    {
        return Color.black;
    }
}