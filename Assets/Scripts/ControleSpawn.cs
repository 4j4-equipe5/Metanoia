using UnityEngine;

/// <summary>
/// Ce script permet de faire téléporter le joueur vers son point de téléportation et appelle la méthode du système d'anomalie
/// Fait par Emile Lucas Wilson
/// </summary>
public class ControleSpawn : MonoBehaviour
{
    private HingeJoint hingePorteSortie; // Référence au HingeJoint de la porte de sortie
    [Header("Paramètres de téléportation")]
    [SerializeField] public Transform pointTeleporte; // Point de téléportation
    [SerializeField] public GameObject joueur; // Référence au joueur
    private Transform joueurTR;
    [SerializeField] public float angleThreshold; // Seuil d'angle pour déclencher la téléportation

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hingePorteSortie = GetComponent<HingeJoint>();
        joueurTR = joueur.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
       if (Mathf.Abs(hingePorteSortie.angle) > angleThreshold) // vérifie si l'angle de la porte dépasse le seuil défini
        {
            TeleporterJoueur(); 
        }
    }
    void TeleporterJoueur()
    {
        // 1. J'empeche le script immédiament
        // Cela empêche l'update de jouer en Boucle
        this.enabled = false;
        // 2. Loquique pour le système d'anomalie ;)
        // Instance la methode StartNewRound : Génere une nouvelle liste d'anomalie
        ControleAIAnomalie.Instance.StartNewRound();
        // 3. Téléportation du joueur
        // Besoin de désactiver le script
        ScriptMouvementPerso cc = joueur.GetComponent<ScriptMouvementPerso>();
        cc.enabled = false;
        // Téléporte vers le point de téléportation
        joueurTR.position = pointTeleporte.position;
        joueurTR.rotation = pointTeleporte.rotation;

        // réactive le script de mouvement
        cc.enabled = true;

        // 4. On réactive le script avec un  délai
        Invoke("ReactiverScript", 5f);
    }
    void ReactiverScript()
    {
        this.enabled = true;
    }
}
