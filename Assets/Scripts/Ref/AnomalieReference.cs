using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Fait par Emile Lucas Wilson 
/// Ce script les refs des anomalies
/// </summary>
public class AnomalieReference : MonoBehaviour
{

    [Header("Les références auz objets")]
    [SerializeField] public List<GameObject> anomaliesObjets; // liste des anomalies à gérer, assignée dans l'inspecteur
    [SerializeField] public List<GameObject> normalObjets; // liste des objets normal à gérer et sert comme valeur initiale pour le cerveau
    [SerializeField] public List<GameObject> miannList;
    [SerializeField] public List<GameObject> miannSpawnPoints;  
    public ScriptGestionPointage gestionPointage; // POUR JÉ 
    public Animator porteAscenseur;
    public SonManager sonManager; // référence au script de gestion du son
    [SerializeField] public StressLighting scriptStressLighting; // référence au script de gestion du stress lighting
    public ScriptMouvementPerso scriptMouvementPerso; // référence au script de gestion des armes
    public dataArmes armeGlock; // données de l'arme Glock à donner au joueur au round 1
    public GameObject prefabPistole; // prefab de la pistole à donner au joueur au round 1
    public ScriptGestionArme scriptGestionArme; // référence au script de gestion des armes pour activer l'arme donnée
    // donnees des armes
    public List<dataArmes> donneesArmes; // liste des données des armes, assignée dans l'inspecteur
    public Transform player;
    [SerializeField] public Transform spawnCorridor; 

    public GameObject porteTeleportation;
    public GameObject porteActive;
    public TeleUI scriptTeleUI; // référence au script de tv du jeu
    public ScriptPorteSortie scriptPorteSortie;
    [Header("Les paramètres")]
    [Range(0, 1)] public float anomalieChance; // chance qu'une anomalie se produise, assignée dans l'inspecteur
    [SerializeField] public bool isAnomalieActive {get; private set; } // indique si une anomalie est actuellement active dans le jeu
    public  int anomalieCount;
    [SerializeField] public int roundsActuel;
    public float angleThreshold;
    public int reponseJoueur;
    public int pointsGagne;
    [SerializeField] public int pointsErreur; // points qui gère les conséquences des erreurs du joueurs

    //TODO: LES REFS DE SCRIPT DE POINTS 

}