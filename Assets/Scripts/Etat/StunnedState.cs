using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

/// <summary>
/// 
/// Fait par Emile Lucas Wilson
/// Cette état met ennemi en stun : 
/// EN STUN
///     - Ennemi va en ragdoll ( force dépendant de l'arme et la direction et plus)
///     - Ennemi prend plus de dégats
/// <summary>
public class StunnedState : IState
{
    //Ref
    private EnnemyReferences _ennemyRef;
    private Transform player;

    Vector3 startPosition;

    // Constructeur
    public StunnedState (EnnemyReferences ennemyRef)
    {
        _ennemyRef = ennemyRef;
        player = ennemyRef.player;
    }

    public void OnEnter()
    {
        // Vérifie que l'agent est valide et sur le NavMesh avant de le arrêter
        if (_ennemyRef.agent != null && _ennemyRef.agent.isOnNavMesh)
        {
            _ennemyRef.agent.isStopped = true; // Arrête le NavMeshAgent pour le Stun
        }
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
        _ennemyRef.rbEnnemi.AddForce(force, ForceMode.Impulse);
        _ennemyRef.sonManager.SonMiann(SonManager.IdSonMiann.Stunned);
    }
    public void Tick()
    {
        // le DamageThreshold revient tranquilement en dessous de trois
        _ennemyRef.damageThreshold -= Time.deltaTime * 0.5f;
        _ennemyRef.damageThreshold = Mathf.Max(0, _ennemyRef.damageThreshold);
        if (_ennemyRef.damageThreshold < 0.5f)
        {
            _ennemyRef.isStunned = false;
        }
    }
    public void OnExit()
    {
        _ennemyRef.agent.enabled = true; // Réactive le NavMeshAgent après l'attaque
        _ennemyRef.agent.isStopped = false; // Permet au NavMeshAgent de reprendre le contrôle du mouvement
        _ennemyRef.animEnnemi.enabled = true; // Réactive l'Animator après l'attaque
        _ennemyRef.forceFreezeHips.enabled = true; // Réactive le script de freeze des hanches après l'attaque
        Debug.Log("Ennemi n'est plus stunned");
    }
    public Color GizmoColor()
    {
        return Color.gray;
    }
}