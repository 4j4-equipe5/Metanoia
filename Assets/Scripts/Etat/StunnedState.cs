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
/// Modification : 
///    - le script plus simple 
/// <summary>
public class StunnedState : IState
{
    //Ref
    private EnnemyReferences _ennemyRef;
    private Transform player;


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
        // Ragdoll : Different 
        foreach (GameObject joint in _ennemyRef.joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false; // Rend les rigidbodies non-kinematic pour permettre au ragdoll de réagir aux forces
        }
        Rigidbody rbHips = _ennemyRef.joints[0].GetComponent<Rigidbody>(); // Supposons que le premier joint est les hanches
        if (rbHips != null)
        {
            Vector3 directionRecul = _ennemyRef.directionDernierImpact; // Direction du recul basée sur le dernier impact
            directionRecul.y = 0.4f; //
            directionRecul.Normalize();

            // Utilise la force de recul enregistrée dans les références de l'ennemi
            rbHips.AddForce(directionRecul * _ennemyRef.forceDernierRecul, ForceMode.Impulse);
        }
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
        foreach (GameObject joint in _ennemyRef.joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = true; // Rend les rigidbodies kinematic pour désactiver le ragdoll
        }
        _ennemyRef.agent.enabled = true; // Réactive le NavMeshAgent après l'attaque
        _ennemyRef.agent.isStopped = false; // Permet au NavMeshAgent de reprendre le contrôle du mouvement
        _ennemyRef.agent.Warp(_ennemyRef.transform.position); // Assure que le NavMeshAgent est à la bonne position après le stun
        _ennemyRef.animEnnemi.enabled = true; // Réactive l'Animator après l'attaque
        _ennemyRef.forceFreezeHips.enabled = true; // Réactive le script de freeze des hanches après l'attaque
        Debug.Log("Ennemi n'est plus stunned");
    }
    public Color GizmoColor()
    {
        return Color.gray;
    }
}