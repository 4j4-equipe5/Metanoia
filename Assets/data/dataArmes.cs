using UnityEngine;

[CreateAssetMenu(fileName = "dataArmes", menuName = "Scriptable Objects/dataArmes")]
public class dataArmes : ScriptableObject
{
    public string nomArme;
    public float degats;
    public float cadence;
    public int maxReserve;
    public GameObject prefabArme;
    public GameObject prefabMuzzleFlash;
}
