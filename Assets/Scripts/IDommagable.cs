using UnityEngine;
/*
ce script contient un interface. Les interfaces sont utiles en raison
du fait que une seule classe parente peut etre heritee. Comme les scripts
unity heritent deja de la classe monobehavior, on utilise un interface
pour qu'elles heritent d'autres comportements. Plus d'infos sur les interfaces :
https://www.w3schools.com/cs/cs_interface.php

en l'occurance, l'interface sert a appliquer des dommages a plusieurs
types d'objets quand le joueur tire dessus. Les ennemis et objets
destructible heriterons donc de IDommagable.
*/
public interface  IDommagable
{
    // Modif par : Emile
    // j'ajoute un RaycastHit en parametre pour que les ennemis puissent réagir différemment selon la partie du corps touchée (ex: headshot vs body shot)
    // + un float pour la force du tir / attaque de monstre, qui peut être utilisé pour appliquer une force de recul plus importante sur les ennemis plus légers ou pour faire tomber des objets destructibles
    public void PrendreDegat(int degats, RaycastHit hit, float forceRecul);
}
