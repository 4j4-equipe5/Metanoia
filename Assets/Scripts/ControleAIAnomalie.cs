using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Ce script permet au gameobject Anomalie-Manager de gérer les différentes anomalies présentes dans le jeu, en fonction de leur type et de leur comportement spécifique.
/// Il peut être utilisé pour contrôler les mouvements, les interactions et les effets des anomalies sur le joueur et l'environnement du jeu.
/// Il peut également être utilisé pour déclencher des événements spécifiques liés aux anomalies, tels que des changements de niveau, des effets visuels ou sonores, ou des interactions avec d'autres éléments du jeu.
/// Fait par : Emile Lucas, 2026-04-21
/// </summary>
public class ControleAIAnomalie : MonoBehaviour
{
    public static ControleAIAnomalie Instance; // Actualiser en Singleton
    [Header("Références aux objets")]
    [SerializeField] public List<GameObject> anomaliesObjets; // liste des anomalies à gérer, assignée dans l'inspecteur
    [SerializeField] public List<GameObject> normalObjets; // liste des objets normal à gérer et sert comme valeur initiale pour le cerveau
    [Header("Paramètres des anomalies")]
    [Range(0, 1)] public float anomalieChance; // chance qu'une anomalie se produise, assignée dans l'inspecteur
    [SerializeField] public bool isAnomalieActive {get; private set; } // indique si une anomalie est actuellement active dans le jeu
    [Header("Paramètre de récompense")]
    [SerializeField] public int pointsDonnés; // points donnés au joueur pour avoir survécu à une anomalie, assigné dans l'inspecteur
    [SerializeField] public int pointsPerdus; // points perdus par le joueur pour avoir été affecté par une anomalie, assigné dans l'inspecteur
    [SerializeField] public int pointsMax; // points maximum que le joueur peut atteindre, assigné dans l'inspecteur
    [SerializeField] private int pointsActuels; // points actuels du joueur, initialisés à zéro au début du jeu

    void Awake()
    {
        if (Instance == null ) Instance = this;
        else Destroy(gameObject); // assure qu'il y a juste un Manager par scène
        pointsActuels = 0; // initialisation des points du joueur à zéro au début du jeu
    }
    void Start()
    {
        ResetRoom();
    }
    public void StartNewRound() // commence une nouvelle salle ou un nouveau niveau, réinitialise les points et les anomalies
    {
        ResetRoom(); // réinitialise les objets de la salle à leur état normal
        isAnomalieActive = Random.value < anomalieChance; // détermine aléatoirement si une anomalie se produira dans cette salle en fonction de la chance définie
        // si il a une anomalie active la fonction TriggerRandomAnomaly : Elle va placer une anomaly dans la salle Anomaly 
        if (isAnomalieActive)
        {
            TriggerRandomAnomaly();
        }
    }
    void ResetRoom() // Cette Fonction réinitialise les objets de la salle a leur état initial, en désactivant et en réactivant !!
    {
        for (int i = 0; i < normalObjets.Count; i++) // boucle à travers tous les objets normaux de la salle
        {
            anomaliesObjets[i].SetActive(false); // désactive les anomalies pour les remettre à leur état initial
            normalObjets[i].SetActive(true); // réactive les objets normaux pour les remettre à leur état initial
        }
    }
     // Cette fonction permet le manager de sélectionner un range random des anomalies dans le project unity
    
    void TriggerRandomAnomaly()
    {
        // index est un intègre un nombre random du nombre d'objet normal dans la salle anomalie... 
        // par exemple il a 10 objet normal 
        // si le nombre est à 8 qui est une horloge 
        // index va selectionner 8, va désactiver l'objet normal et activer l'horloge bizzare
        int index = Random.Range(0, normalObjets.Count); 

        normalObjets[index].SetActive(false);
        anomaliesObjets[index].SetActive(true);

        Debug.Log($"Anomaly activé: {anomaliesObjets[index].name}");
    }
}
