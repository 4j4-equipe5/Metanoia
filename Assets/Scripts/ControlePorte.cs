using UnityEngine;
using System.Collections.Generic;
/// <summary> 
/// Ce script permet de contrôler le comportement des portes dans le jeu, y compris leur ouverture, leur fermeture et leur interaction avec le joueur.
/// Il peut être utilisé pour créer des mécanismes de puzzle, des passages secrets ou des obstacles à franchir.
/// Fait par : Emile Lucas, 2026-04-21
/// </summary>

public class ControlePorte : MonoBehaviour
{
    private Rigidbody rbPorte;
    [SerializeField] public bool porteBarrer = true; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rbPorte = GetComponent<Rigidbody>();
        rbPorte.isKinematic = porteBarrer;
    }
    public void UnlockPorte() // fonction pour ouvrir la porte, appelée par d'autres scripts ou événements dans le jeu
    {
        porteBarrer = false;
        rbPorte.isKinematic = false; // rend la porte non cinématique pour permettre son mouvement
    }

}
