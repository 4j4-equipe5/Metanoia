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
    public enum IdSonSam {GradeA, GradeB, GradeC, Error, Parfait, Arme, Shotgun, erreurMoyen, erreurMajeur, Fin, GameOver}
    [Header("Loto sound")]
    public List<AudioClip> sonLoto;

    private void Start()
    {
        speakerSon = GetComponent<AudioSource>();
    }
    public void SamSon(IdSonSam identifiant)
    {
        switch (identifiant)
        {   
            case IdSonSam.GradeA:
            speakerSon.PlayOneShot(sonSam[0]);
            break;
            case IdSonSam.GradeB:
            speakerSon.PlayOneShot(sonSam[1]);
            break;
            case IdSonSam.GradeC:
            speakerSon.PlayOneShot(sonSam[2]);
            break;
            case IdSonSam.Error:
            speakerSon.PlayOneShot(sonSam[3]);
            break;
            case IdSonSam.Parfait:
            speakerSon.PlayOneShot(sonSam[4]);
            break;
            case IdSonSam.Arme:
            speakerSon.PlayOneShot(sonSam[5]);
            break;
            case IdSonSam.Shotgun:
            speakerSon.PlayOneShot(sonSam[6]);
            break;
            case IdSonSam.erreurMoyen:
            speakerSon.PlayOneShot(sonSam[7]);
            break;
            case IdSonSam.erreurMajeur:
            speakerSon.PlayOneShot(sonSam[8]);
            break;
            case IdSonSam.Fin:
            speakerSon.PlayOneShot(sonSam[9]);
            break;
            case IdSonSam.GameOver:
            speakerSon.PlayOneShot(sonSam[10]);
            break;
        }
    }
}
