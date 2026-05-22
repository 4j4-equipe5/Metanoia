using UnityEngine;
using TMPro;

public class scriptStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text vieUI;
    [SerializeField] private TMP_Text apUI;
    [SerializeField] private TMP_Text ammoUI;

    private int apAffiche = 0;
    private int vieAffiche = 0;
    private int ammoAffiche = 0;

    private ScriptMouvementPerso joueur;
    private ScriptGestionArme arme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joueur = ScriptMouvementPerso.Instance;
        arme = ScriptGestionArme.Instance;
    }

    // Update is called once per frame
    void Update()
    {
       apUI.text = joueur.apPlayer.ToString();
       vieUI.text = joueur.hpPlayer.ToString();
        ammoUI.text = arme.reserveMunitionActuelle.ToString();
    }
}
