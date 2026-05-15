using UnityEngine;

[CreateAssetMenu(fileName = "dataBuffs", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class dataBuffs : ScriptableObject
{
    public BuffDebuff type;

    public string nomAffichage;

    public float value;
}
