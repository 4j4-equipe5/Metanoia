using UnityEngine;
//systeme de query avec syntaxe similaire a sql pour gerer des donnees
//documentation : https://learn.microsoft.com/en-us/dotnet/api/system.linq?view=net-10.0
using System.Linq;

public class MachineBonus : MonoBehaviour, IInteraction
{
    [SerializeField] private dataBuffs[] buffsPossibles;
    public string InteractionLabel {get;} = "Tentez votre chance [1000]";
    public int prix {get;} = 1000;

    /// <summary>
    /// classe Interagir qui est necessaire pour l'interface IInteraction
    /// </summary>
    /// <param name="joueur">script gestion du joueur</param>
    public void Interagir(ScriptMouvementPerso joueur)
    {
        //en utilisant System.Linq, on fait une query sur la liste des buff possibles
        //qui sont definis dans l'inspecteur a partir des fichiers data.
        // la ou le retour de BuffPresent (voir script joueur) est faux, on
        //ajoute ce buff au tableau disponibles
        var disponibles = buffsPossibles.Where(b => !joueur.BuffPresent(b.type)).ToArray();
        //si tous les buffs sont indispo, on ne fait rien
        if(disponibles.Length == 0) return;
        //on donne un buff aleatoire dans la liste
        var buffChoisi = disponibles[Random.Range(0, disponibles.Length)];
        //appel classe AppliquerBuff dans scriptgestionjoueur avec le buffchoisi en argument
        joueur.AppliquerBuff(buffChoisi);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
