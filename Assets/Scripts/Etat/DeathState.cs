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
    private float ragdollForce; // Force du ragdoll sur le tir
    public void OnEnter(){}

    public void Tick(){}
    public void OnExit(){}
    public Color GizmoColor()
    {
        return Color.black;
    }
}