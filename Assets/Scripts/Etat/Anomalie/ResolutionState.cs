using UnityEngine;

/// <summary>
/// Fait par Emile Lucas Wilson ce script permet :
///     - Controle le son du narrateur : SAM
///     - les pointss donné 
///     - les erreurs
///     - Tp du joueur quand il ouvre la porte à un certain angleThreshold
///     - JOUER SON SAM 
/// </summary>
public class ResolutionState : IState
{
    //Ref
    private AnomalieReference _anomalieRef;
    public bool isComplete {get; private set;}

    //Constructeur
    public ResolutionState (AnomalieReference anomalieRef)
    {
        _anomalieRef = anomalieRef;
    }

    public void OnEnter()
    {
        isComplete = false;
        _anomalieRef.roundsActuel ++;
        // permet d'ouvrir la porte
        _anomalieRef.scriptPorteSortie.AutoriserOuverture();
        // Verification de la reponse du Joueur
        if (_anomalieRef.reponseJoueur == _anomalieRef.anomalieCount)
        {
            // Joueur reçois des points
            // TODO: POINTS et SAM
            _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.GradeA);
            _anomalieRef.scriptStressLighting.SetStressLevel(0); // reset le stress lighting calme
            //Points Gagner * 3
        }
        else
        {
            // Le joueur s'est trompé : On calcule la différence
            // Exemple : Mathf.Abs(5 - 7 ) donne 2 erreurs
            int pointsErreurGagnes = Mathf.Abs(_anomalieRef.reponseJoueur - _anomalieRef.anomalieCount);
            _anomalieRef.pointsErreur += pointsErreurGagnes;
            switch (pointsErreurGagnes)
            {
                case 1:
                // SAM ANNONCE RANK B
                //Points Gagner
                _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.GradeB);
                // points fois 2
                break;
                case 2:
                // SAM ANNONCE RANK C
                //Points Gagner 
                _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.GradeC);
                break;
                case 3:
                case 4:
                case 5:
                // SAM Son erreur
                    _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.Error);
                break;
                case 6: 
                // Sam Son erreur grave
                    _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.erreurMoyen);
                    break;
                case 7:
                default:
                // Sam Son erreur majeur
                    _anomalieRef.sonManager.SamSon(SonManager.IdSonSam.erreurMajeur);
                //SON SAM DANGER
                break;
                
            }
        }
    }
    public void Tick()
    {
        // si la méthode est activer : teleporte le joueur
        if (_anomalieRef.scriptPorteSortie.AtteintAngleThresold())
        {
            TeleporteJoueur();
            isComplete = true;
        }
    }
    public void OnExit()
    {
        // reset la porte pour empecher de spam TP
        _anomalieRef.scriptPorteSortie.ReinitialiserPorte();
    }

    /// <summary>
    /// Cette fonction permet de TP le joueur vers le corridor
    ///     -
    /// </summary>
    private void TeleporteJoueur()
    {
        // Voir si on doit déactiver le script de mouvement

        // TP le joueur vers le points de TP (dans le corridor)
        _anomalieRef.player.position = _anomalieRef.spawnCorridor.position;

    }

    public Color GizmoColor()
    {
        return Color.red;
    }
}