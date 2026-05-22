using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class scriptStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text vieUI;
    [SerializeField] private TMP_Text apUI;
    [SerializeField] private TMP_Text ammoUI;
    [SerializeField] private Image[] backgroundArme;


    private ScriptMouvementPerso joueur;
    
    private Color couleurActive = new Color(233f/255f, 187f/255f, 187f/255f, 1f);
    private Color couleurDefaut = new Color(32f/255f, 5f/255f, 5f/255f, 1f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joueur = ScriptMouvementPerso.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        apUI.text = joueur.Armure.ToString();
        vieUI.text = joueur.Vie.ToString();
        ammoUI.text = joueur.MunActuelle.ToString();

        for(int i = 0; i < backgroundArme.Length; i++)
        {
           backgroundArme[i].color = i == joueur.IndexArmeActive ? couleurActive:couleurDefaut;
        }

    }
}
