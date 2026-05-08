using UnityEngine;
using TMPro;
/// <summary>
/// Script attache a l'objet parent du compteur score dans le HUD. Il
/// affiche le score que ScriptGestionPointage lui passe par le biais
/// de ses proprietes
/// </summary>
public class UIscore : MonoBehaviour
{

    [SerializeField] TMP_Text texteScore;
    [SerializeField] TMP_Text texteCombo;
    [SerializeField] TMP_Text texteMultiplicateur;
    
    private float scoreAffiche = 0;
    private float comboAffiche = 0;
    private float multiAffiche;
    private float vitesseAnim = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //variables float qui permettent d'interpoler entre les scores
        //pour une petite anim
        multiAffiche = Mathf.Lerp(multiAffiche,ScriptGestionPointage.Instance.multi,vitesseAnim * Time.deltaTime);
        scoreAffiche = Mathf.Lerp(scoreAffiche,ScriptGestionPointage.Instance.score,vitesseAnim * Time.deltaTime);
        comboAffiche = Mathf.Lerp(comboAffiche, ScriptGestionPointage.Instance.combo,vitesseAnim * Time.deltaTime);

        texteScore.text = $"{Mathf.RoundToInt(scoreAffiche)}";
        texteCombo.text = $"{Mathf.RoundToInt(comboAffiche)}";
        texteMultiplicateur.text = $"{Mathf.RoundToInt(multiAffiche)}X";
    }
}
