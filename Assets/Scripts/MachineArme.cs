using UnityEngine;

public class MachineArme : MonoBehaviour, IInteraction
{
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
        
    }
}
