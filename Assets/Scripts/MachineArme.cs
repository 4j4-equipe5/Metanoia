using UnityEngine;
using System.Linq;
public class MachineArme : MonoBehaviour, IInteraction
{
    [SerializeField] private dataArmes[] armesPossibles;
    public string InteractionLabel {get;} = "Obtenir une arme [20000]";
    public int prix {get;} = 20000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interagir(ScriptMouvementPerso joueur)
    {
        var disponibles = armesPossibles.Where(a => !joueur.ArmePresente(a.nomArme)).ToArray();
        if(disponibles.Length == 0) return;

        dataArmes armeChoisi = disponibles[Random.Range(0,armesPossibles.Length)];
        joueur.ObtenirArme(armeChoisi);
    }
}
