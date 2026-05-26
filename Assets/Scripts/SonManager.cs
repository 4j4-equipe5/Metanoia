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
    public List<AudioClip> sonLoto = new List<AudioClip>();
    [Header("SonMiann")]
    public List<AudioClip> sonMiann = new List<AudioClip>();
    public enum IdSonMiann {Spawn, Attaque, Projectile, Saut, FinSaut, Stunned, Mort}

    private void Awake()
    {
        speakerSon = GetComponent<AudioSource>();
        if (speakerSon == null)
        {
            speakerSon = GetComponent<AudioSource>();

            // Sécurité ultime : si aucun AudioSource n'est attaché sur l'objet, on en ajoute un pour éviter le crash
            if (speakerSon == null)
            {
                speakerSon = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning($"[SonManager] Aucun composant AudioSource trouvé sur {gameObject.name}. Un composant a été ajouté automatiquement pour éviter le crash.");
            }
        }
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
    public void SonMiann(IdSonMiann identifiant)
    {
        switch (identifiant)
        {
            case IdSonMiann.Spawn:
            speakerSon.PlayOneShot(sonMiann[0]);
            break;
            case IdSonMiann.Attaque:
            speakerSon.PlayOneShot(sonMiann[1]);
            break;
            case IdSonMiann.Projectile:
            speakerSon.PlayOneShot(sonMiann[2]);
            break;
            case IdSonMiann.Saut:
            speakerSon.PlayOneShot(sonMiann[3]);
            break;
            case IdSonMiann.FinSaut:
            speakerSon.PlayOneShot(sonMiann[4]);
            break;
            case IdSonMiann.Stunned:
            speakerSon.PlayOneShot(sonMiann[5]);
            break;
            case IdSonMiann.Mort:
            speakerSon.PlayOneShot(sonMiann[6]);
            break;
            
        }
    }
}
