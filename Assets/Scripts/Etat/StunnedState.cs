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
    private int bonusDMG;
    private float timePassed;

    Vector3 startPosition;

    // Constructeur
    public StunnedState (EnnemyReferences ennemyRef)
    {
        _ennemyRef = ennemyRef;
        player = ennemyRef.player;
    }

    public void OnEnter()
    {
        _ennemyRef.agent.isStopped = true; // Arrête le NavMeshAgent pour le Stun
        _ennemyRef.agent.enabled = false; // Désactive le NavMeshAgent pour permettre un contrôle total de l'ennemi pendant le Stun

        Debug.Log("Ennemi est stunned");

        startPosition = _ennemyRef.transform.position;
        // On veut récupérer la Direction pour le ragdoll suit la direction du joueur
        Vector3 direction = (player.position - startPosition).normalized;
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
    }
    public Color GizmoColor()
    {
        return Color.gray;
    }
}