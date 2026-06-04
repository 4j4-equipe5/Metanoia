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
        // Son de mort
        _ennemyRef.sonManager.SonMiann(SonManager.IdSonMiann.Mort);
        
        Debug.Log("Ennemi est mort");
        startPosition = _ennemyRef.transform.position;
        // On veut récupérer la Direction pour le ragdoll suit la direction du joueur
        Vector3 direction = (startPosition - player.position).normalized;

        // normaliser la direction en y
        direction.y = 0;
        direction.Normalize();

        // Ajout du recul pour projeter l'ennemi vers le haut
        direction.y = 0.7f; // Placeholder, à ajuster selon les besoins pour la composante verticale du recul
        direction.Normalize();
        // Applique la force de ragdoll à tous les joints de l'ennemi
        foreach (GameObject joint in _ennemyRef.joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = false; // Rend les rigidbodies non-kinematic pour permettre au ragdoll de réagir aux forces

        }
        Rigidbody rbHips = _ennemyRef.joints[0].GetComponent<Rigidbody>(); // Supposons que le premier joint est les hanches
        if (rbHips != null)
        {
            Vector3 directionRecul = _ennemyRef.directionDernierImpact; // Direction du recul basée sur le dernier impact
            directionRecul.y = 4f; //
            directionRecul.Normalize();

            // Utilise la force de recul enregistrée dans les références de l'ennemi
            rbHips.AddForce(directionRecul * _ennemyRef.forceDernierRecul * 2, ForceMode.Impulse);
        }
        // TODO: Ajout des Points
        timePassed = 0f;
    }

    public void Tick()
    {
        timePassed += Time.deltaTime;

        if (timePassed >= 1f && !_ennemyRef.isDead)
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