using UnityEngine;
using TMPro;

/// <summary>
/// Fait par Emile Lucas Wilson
/// 
/// Ce script permet de mettre À jour l'affichage de la tv 
///     - plus permet au ajouter
///     - moins permet de réduire
///     - Enter peremt de envoyer la r.ponse du joueur
/// Et quand fini la télé est reset
/// TODO: AJOUT SON
/// </summary>
public class TeleUI : MonoBehaviour
{
    [Header("Paramètres de Surveillance")]
    public int numeroPiece = 0;
    public bool choixFait = false;

    [Header("Composants UI de la Télé")]
    [SerializeField] private TextMeshProUGUI texteAffichagePiece; 

    [Header("Sécurité d'Interaction")]
    [Tooltip("Glisse ici le conteneur ou la liste des objets enfants d'interaction pour les désactiver.")]
    [SerializeField] private GameObject[] zonesInteractionBoutons;

    private void Start()
    {
        MettreAJourEcran();
    }

    public void ActionBoutonPlus()
    {
        numeroPiece++;
        MettreAJourEcran();
        Debug.Log("PLUS");
    }

    public void ActionBoutonMoins()
    {
        if (numeroPiece > 0)
        {
            numeroPiece--;
            MettreAJourEcran();
        }
    }

    public void ActionBoutonEnter()
    {
        if (choixFait) return;
        Debug.Log($"[Télé] Choix final validé pour la pièce : {numeroPiece}. Désactivation des commandes.");
        
        // C'est ici qu'on applique le verrouillage par une méthode
        // choixFait qui va être pris par le cerveau Anomalie
        choixFait = true;
        SetEtatBoutons(false);
    }

    private void MettreAJourEcran()
    {
        if (texteAffichagePiece != null)
        {
            texteAffichagePiece.text = numeroPiece.ToString();
        }
    }


    /// <summary
    /// Fonction Essentielle applée par GenererRound pour remmettre la télé a un état neuf
    /// </summary
    public void ResetTele()
    {
        numeroPiece = 0;
        choixFait = false;
        SetEtatBoutons(true); // ReActive les boutons
        MettreAJourEcran();
    }

    /// <summary>
    /// Cette fonction permet d'activer ou désactiver les boutons
    /// Ensuite, la boucle permet d'intéragir avec les boutons
    /// </summary>
    /// <param name="activer"></param>
    private void SetEtatBoutons(bool activer)
    {
        if (zonesInteractionBoutons == null) return;

        foreach (GameObject zone in zonesInteractionBoutons)
        {
            if (zone != null)
            {
                zone.SetActive(activer);
            }
        }
    }
}