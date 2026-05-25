using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary>
/// Fait par Emile 
/// Ce script gère le son de la scène grâçe à des Switch
/// </summary>
public class SonManager : MonoBehaviour
{
    //Ref
    private AudioSource speakerSon;
    [Header("SAM SON")]
    public List<AudioClip> sonSam = new List<AudioClip>(); // 10 clip de sam
    public enum IdSonSam {GradeA, GradeB, GradeC, Error, Parfait, Arme, Shotgun, erreurMoyen, erreurMajeur, Fin}
    [Header("Loto sound")]
    public List<AudioClip> sonLoto;

    public void SamSon(IdSonSam identifiant)
    {
        
    }
}
