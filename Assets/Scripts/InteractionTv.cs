using UnityEngine;

/// <summary>
/// Fait par Emile Lucas Wilson
/// 
/// Ce script peremet d'intéragir avec les boutons de la télé graçe au layers et le raycast IInteraction
///     - Plus
///     - Moins
///     - Enter
/// </summary>
public class InteractionTv : MonoBehaviour, IInteraction
{
    //ref
    public enum TypeBouton {Plus, Moins, Enter}

    [Header("Configuration des boutons")]
    [SerializeField] private TeleUI scriptTele; // Glisse le script principal de ta TV ici
    [SerializeField] private TypeBouton fonctionDuBouton; 
    [SerializeField] private string labelAffiche = "Interagir avec le bouton";

    public string InteractionLabel => labelAffiche;

    // L'action est gratuite
    public int prix => 0;

    /// <summary>
    /// Appelé le raycast du joueur quand le joueur regarde le bouton enfant
    /// </summary>
    public void Interagir(ScriptMouvementPerso joueur)
        {
            if (scriptTele == null)
            {
                Debug.LogError($"Il manque la référence au script TeleUI sur l'objet {gameObject.name} !");
                return;
            }

            // On aiguille l'action selon le type choisi dans l'inspecteur :
            // Utilise Switch pour un code plus propre et court
            switch (fonctionDuBouton)
            {
                case TypeBouton.Plus:
                    scriptTele.ActionBoutonPlus();
                    break;
                case TypeBouton.Moins:
                    scriptTele.ActionBoutonMoins();
                    break;

                case TypeBouton.Enter:
                    scriptTele.ActionBoutonEnter();
                    break;
            }
        }
}
