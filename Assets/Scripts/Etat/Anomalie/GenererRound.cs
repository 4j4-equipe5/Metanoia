using UnityEngine;
using System.Collections.Generic;


///<summary>
/// Fait par Emile
/// 
/// Ce script génère les rounds :
///     - si le joueur fait 3 erreurs ou plus spawn des ennemies 
///     - Random spawn des ennemies
///     - Génère les anomalies
///     - téléporte le joueur vers la salle initial
///     - si le temps, fait que beaucoups d'erreur boost les stats des ennemis
/// </summary>
public class GenererRound : IState
{
    //ref
    private AnomalieReference _anomalieRef;
    public bool isComplete {get; private set; }

    // Constructeur
    public GenererRound(AnomalieReference anomalieRef)
    {
        _anomalieRef = anomalieRef;
    }

    public void OnEnter()
    {   
        // 1. Reset de la piece et la tv
        isComplete = false;
        _anomalieRef.anomalieCount = 0;
        _anomalieRef.reponseJoueur = 0;
        ResetRoom(); // Nettoir la pièce de des anomalies du round précédent
        _anomalieRef.scriptTeleUI.ResetTele();


        // 2. Gestion du danger
        _anomalieRef.roundsActuel ++;
        // Donne une arme au joueur pour le round
        if (_anomalieRef.roundsActuel >= 2)
        {   
            // TODO: Donner une arme au joueur pour le round 2 et plus, par exemple une arme plus puissante que la Glock du round 1
            // _anomalieRef.scriptMouvementPerso.ObtenirArme(_anomalieRef.armeGlock);
            // _anomalieRef.prefabPistole.SetActive(true);
            // _anomalieRef.scriptGestionArme.estObtenue = true;
            _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.Arme);
        }

        GererSpawnEnnemis();
        // La lumière de stress augmente en fonction du nombre d'erreur que le joueur a accumulé, par exemple :
        switch(_anomalieRef.pointsErreur)
        {
            case 0:
            case 1:
            case 2:
                _anomalieRef.scriptStressLighting.SetStressLevel(0); // stress lighting niveau 0
                break;
            case 3:
                _anomalieRef.scriptStressLighting.SetStressLevel(1); // stress lighting niveau 1
                break;
            case 4:
            case 5:
                _anomalieRef.scriptStressLighting.SetStressLevel(2); // stress lighting niveau 2
                break;
            case 6:
                _anomalieRef.scriptStressLighting.SetStressLevel(3); // stress lighting niveau 3
                break;
            case 7:
            default:
                _anomalieRef.scriptStressLighting.SetStressLevel(4); // stress lighting niveau max
                 break;
        }
        // 3. Gestion des anomalies... décide qu'elle anomalie spawn
        SpawnAnomalie();

        // 4. il est finit
        isComplete = true;
    }
    public void Tick()
    {
        // Pas besoins
    }
    // Cette méthode réinitialise les objets de la salle a leur état initial
    void ResetRoom()
    {
        for (int i = 0; i < _anomalieRef.anomaliesObjets.Count; i++) // boucle èa travers tout les objets anomalie
        {
            _anomalieRef.anomaliesObjets[i].SetActive(false);    // désactive les anomalies pour les remettre à leur état initial
            _anomalieRef.normalObjets[i].SetActive(true);    // désactive les anomalies pour les remettre à leur état initial
        }
    }

    void GererSpawnEnnemis()
    {
        // CONDITION A = Le joueur a accumulé 3 erreur ou plus -> PUNITION 
        // initialise une boucle qui va spawn des ennemis par le nombre d'erreur que le joueur fait
        if (_anomalieRef.pointsErreur >= 3)
        {
            Debug.Log("!!! DANGER : MIANN SPAWN !!!");
            int nombreSpawn = _anomalieRef.pointsErreur - 2; // spawn 1 = 3 erreurs, 2 = 4 erreurs, etc

            // Liste temporaire pour distribuer les ennemis sur des spawn points différents
            List<GameObject> pointsDisponibles = new List<GameObject>(_anomalieRef.miannSpawnPoints);

            // Boucle pour spawn des ennemies dans des positions différentes
            for (int i = 0; i < nombreSpawn; i++)
            {
                // Choix d'un point d'apparition au hasard parmi ceux restants
                int indexSpawnAleatoire = UnityEngine.Random.Range(0, pointsDisponibles.Count);
                GameObject pointChoisi = pointsDisponibles[indexSpawnAleatoire];

                // Appel de la fonction de spawn en lui passant le point choisi
                SpawnEnnemiAleatoire(pointChoisi.transform.position);

                // On retire ce point de la liste pour le prochain monstre de la boucle
                pointsDisponibles.RemoveAt(indexSpawnAleatoire);
            }
        }
    }
    /// <summary>
    /// Spawn un ennemi aléatoire positionner par la position choisi
    /// Chaque ennemi vont avoir des paramètres différents décidé par la liste de 5 miann (pas super différent)
    /// </summary>
    void SpawnEnnemiAleatoire(Vector3 positionDeSpawn)
    {
        int randomEnnemy = Random.Range(0, _anomalieRef.miannList.Count); // Choisit au hasard le prefabs configurés
        
        GameObject nouveauMonstre = MonoBehaviour.Instantiate( _anomalieRef.miannList[randomEnnemy], positionDeSpawn,Quaternion.identity); // initialise le prefab vers un spawn random
        nouveauMonstre.SetActive(true);
        _anomalieRef.sonManager.SonMiann(SonManager.IdSonMiann.Spawn);

    }

    /// <summary>
    /// Gère la sélection et le nombre d'activation aléatoire du nombre anomalie ce round
    /// </summary>
    void SpawnAnomalie()
    {
        // 1. paramètre des anomalies : 
        //      - Le nombre de tentative, le script cherche 10 mais si il a moins il va choisir un nombre déterminer
        //      par la liste et deviens le maximum
        int maxAnomaliePossible = Mathf.Min(10, _anomalieRef.anomaliesObjets.Count);
        int nbAnomaliesTentes = Random.Range(1, maxAnomaliePossible + 1); // 1 à 10

        // 2. Préparation des anomalies anti-doublon
        List<int> indexDisponibles = new List<int>();
        for (int i = 0; i < _anomalieRef.anomaliesObjets.Count; i++)
        {
            indexDisponibles.Add(i);
        }

        // 4. Boucle de génération du système par le nombre tentative max alors par exemple 
        //      - 3 tentative selectionner 
        //      - 
        for (int i = 0; i < nbAnomaliesTentes; i++)
        {
            // A. Choisis un objet random : la chaise qui peut avoir du sang
            int choixAleatoire = Random.Range(0, indexDisponibles.Count);
            int indexObjet = indexDisponibles[choixAleatoire];

            //B. le roll qui décide si elle va devenir une anomalie et compte le nb d'anomale dans le round
            float roll = Random.Range(0f, 1f);
            if (roll <= _anomalieRef.anomalieChance)
            {
                _anomalieRef.anomaliesObjets[indexObjet].SetActive(true);
                _anomalieRef.normalObjets[indexObjet].SetActive(false);
                _anomalieRef.anomalieCount ++; 
            }
            // C. Emppêche des doublons
            indexDisponibles.RemoveAt(choixAleatoire);
        }
        Debug.Log($"[GÉNÉRATEUR] Tentatives : {nbAnomaliesTentes}. Anomalies actives ce round : {_anomalieRef.anomalieCount}.");

    }
    public void OnExit()
    {
    }
    public Color GizmoColor()
    {
        return Color.blue;
    }

}