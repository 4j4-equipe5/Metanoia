using UnityEngine;

public class ScriptGestionPointage : MonoBehaviour
{
    [Header("Score")]
    private static int scoreTotal = 0;
    private int pointsKillEnnemi = 100;
    private int pointsAnomalie = 500;
    public static ScriptGestionPointage Instance;
    [Header("Combo")]
    private static int incrementCombo = 0;
    private float delaiMaxCombo = 4f;

    private static float dernierKill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
       Instance = this ;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(incrementCombo > 0 && Time.time > delaiMaxCombo + dernierKill)
        {
            incrementCombo = 0;
        }
    }
    /// <summary>
    /// fonction appelee par les ennemis quand ils meurent pour calculer
    /// le pointage
    /// </summary>
    public void EnregistrerKill()
    {
        incrementCombo ++;
        dernierKill = Time.time;

        float multiplicateur = CalculerMultiplicateur();
        int pointsGagnes = (int)(pointsKillEnnemi * multiplicateur);
        scoreTotal += pointsGagnes;

    }
    /// <summary>
    /// calculer le multiplicateur a appliquer en fonction de la valeur d'increment combo
    /// </summary>
    public float CalculerMultiplicateur()
    {
        if(incrementCombo >= 3)return 2f;
        if(incrementCombo >= 6) return 4f;
        if(incrementCombo >= 10) return 8f;
        return 1f;
    }
    public void PointageAnomalie(int nbAnomalieTrouvee)
    {
        int pointsGagnes = nbAnomalieTrouvee * pointsAnomalie;
        scoreTotal += pointsGagnes;
    }
    /// <summary>
    /// fonction qui retire les points au joueur apres achat. Appelee
    /// dans le script des machines
    /// </summary>
    /// <param name="valeurRetiree"></param>
        public void RetirerPoint(int valeurRetiree)
    {
       scoreTotal -= valeurRetiree; 
    }
    //proprietes que le HUD va aller chercher sans toucher au variables privees
    public int score => scoreTotal;
    public int combo => incrementCombo;
    public int multi => (int)CalculerMultiplicateur();


}
