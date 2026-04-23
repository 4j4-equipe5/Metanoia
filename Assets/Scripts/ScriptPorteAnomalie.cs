using UnityEngine;
using System.Collections.Generic;
public class ScriptPorteAnomalie : MonoBehaviour
{
    private Rigidbody rbPorte;
    private HingeJoint hingePorte; // component Hingejoint permet d'analyser la angle
    private bool estTrigger = false; 
    [SerializeField] public float angleThreshold;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hingePorte = GetComponent<HingeJoint>();
        rbPorte = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (estTrigger || rbPorte.isKinematic) return; // si la porte est déjà déclenchée ou si elle est encore barrée, ne rien faire
        if (Mathf.Abs(hingePorte.angle) > angleThreshold) // vérifie si l'angle de la porte dépasse le seuil défini
        {
            ActiveSystemAnomlie();
        }
    }
    void ActiveSystemAnomlie()
    {
        estTrigger = true;
        ControleAIAnomalie.Instance.StartNewRound(); // commence la scnène anomalie
    }
}
