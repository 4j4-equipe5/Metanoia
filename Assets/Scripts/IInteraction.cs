using UnityEngine;
/// <summary>
/// interface commun aux objets avec lesquels le joueur peut interagir.
/// a mettre sur les machines, les interrupteurs, les compteurs anomalies,
/// etc...
/// </summary>
public interface  IInteraction
{
    //utilisation d'une propriete en mode lecture. Cela permet un acces
    //sans la possibilite de modification comme dans un champ public.
    //documentation sur les proprietes : https://www.w3schools.com/cs/cs_properties.php
    string InteractionLabel{get;}
    void Interagir(ScriptMouvementPerso joueur);
    int prix {get;}
}
