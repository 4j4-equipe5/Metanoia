using UnityEngine;

/// <summary>
/// Fait par Emile Lucas Wilson
/// Gère l'ouverture de la porte via un composant HingeJoint.
/// </summary>
[RequireComponent(typeof(HingeJoint))]
public class ScriptPorteSortie : MonoBehaviour
{
    [Header("Configurations de l'Angle")]
    [Tooltip("L'angle (en degrés) à atteindre pour déclencher le changement de round / TP.")]
    [SerializeField] private float angleThreshold = 45f;

    [Tooltip("L'angle maximal auquel la porte peut s'ouvrir au total.")]
    [SerializeField] private float angleMaxOuverture = 90f;

    private HingeJoint _hingeJoint;
    private Rigidbody _rigidbody;
    private bool peutEtreOuverte = false;

    private void Awake()
    {
        _hingeJoint = GetComponent<HingeJoint>();
        _rigidbody = GetComponent<Rigidbody>();

        // On s'assure que les limites sont activées sur le joint
        _hingeJoint.useLimits = true;

        // On commence la partie verrouillée
        VerrouillerPortePhysique();
    }

    /// <summary>
    /// Déverrouille la porte en libérant les limites du HingeJoint.
    /// Appelé par ResolutionState.OnEnter().
    /// </summary>
    public void AutoriserOuverture()
    {
        peutEtreOuverte = true;

        // On configure les limites pour permettre l'ouverture
        JointLimits limites = _hingeJoint.limits;
        limites.min = 0f;
        limites.max = angleMaxOuverture;
        _hingeJoint.limits = limites;

        Debug.Log("[PORTE] HingeJoint déverrouillé. Le joueur peut pousser la porte.");
    }

    /// <summary>
    /// Vérifie si la porte a été poussée au-delà de l'angle seuil.
    /// Appelé par ResolutionState.Tick().
    /// </summary>
    public bool AtteintAngleThresold()
    {
        if (!peutEtreOuverte) return false;

        // joint.angle nous donne directement l'angle actuel de rotation du joint
        float angleActuel = Mathf.Abs(_hingeJoint.angle);

        return angleActuel >= angleThreshold;
    }

    /// <summary>
    /// Réinitialise la physique de la porte, la referme et la verrouille.
    /// Appelé par ResolutionState.OnExit().
    /// </summary>
    public void ReinitialiserPorte()
    {
        peutEtreOuverte = false;

        // 1. On remet la porte en position fermée (vitesse et position à zéro)
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        // On force le retour à la rotation d'origine du transform local
        transform.localRotation = Quaternion.identity;

        // 2. On reverrouille le HingeJoint
        VerrouillerPortePhysique();

        Debug.Log("[PORTE] La porte physique est refermée et verrouillée.");
    }

    /// <summary>
    /// Bloque les limites du HingeJoint à 0 pour empêcher tout mouvement.
    /// </summary>
    private void VerrouillerPortePhysique()
    {
        JointLimits limites = _hingeJoint.limits;
        limites.min = 0f;
        limites.max = 0f; // En mettant min et max à 0, le joint ne peut plus bouger
        _hingeJoint.limits = limites;
    }
}