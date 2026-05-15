using UnityEngine;
using TMPro;
/// <summary>
/// script qui gere l'affichage d'un panneau sur le hud quand une interaction est dispo
/// </summary>
public class InteractionUI : MonoBehaviour
{
    public static InteractionUI instance;
    [SerializeField] GameObject panneauPrompt;
    [SerializeField] TMP_Text textePrompt;
    /// <summary>
    /// affiche le panneau prompt avec le label
    /// </summary>
    /// <param name="label">texte qui est affiche et qui change en fonction de l'objet interactif.
    /// Voir l'Interface IInteraction.
    /// </param>
    public void Afficher(string label)
    {
        panneauPrompt.SetActive(true);
        textePrompt.text = $"[E] {label}";
        
    }
    /// <summary>
    /// le fait disparaitre
    /// </summary>
    public void Invisible () => panneauPrompt.SetActive(false);
    void Awake()
    {
        instance = this;
    }

}
