using UnityEngine;

public class ForceFreezeHips : MonoBehaviour
{
    [Header("Sélectionne les deux objets depuis ta Hierarchy :")]
    public Transform hipsNormalBone;
    public Transform hipsControlObject;

    // Variables pour stocker la hauteur (Y) d'origine de ton animation
    private float initialNormalY;
    private float initialControlY;

    void Start()
    {
        if (hipsNormalBone != null) initialNormalY = hipsNormalBone.localPosition.y;
        if (hipsControlObject != null) initialControlY = hipsControlObject.localPosition.y;
    }

    void LateUpdate()
    {
        // Force l'os normal à rester à 0 en X et Z (mouvement au sol)
        if (hipsNormalBone != null)
        {
            hipsNormalBone.localPosition = new Vector3(0f, hipsNormalBone.localPosition.y, 0f);
        }

        // Force le contrôleur à rester à 0 en X et Z
        if (hipsControlObject != null)
        {
            hipsControlObject.localPosition = new Vector3(0f, hipsControlObject.localPosition.y, 0f);
        }
    }
}