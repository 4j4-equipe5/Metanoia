using UnityEngine;
using UnityEngine.UI;

public class SliderSensitivity : MonoBehaviour
{
    public Slider SensitivitySlider;
    private ScriptMouvementPerso joueur;
    private float defaultSensitivity = 0.5f; // valeur de défaut

    void Start()
    {
        joueur = FindFirstObjectByType<ScriptMouvementPerso>();

        if (joueur != null)
        {
            // Utilise la sensi de 0.5 par défaut
            defaultSensitivity = joueur.sensitivityX;
        }

        // Met la sensi entre 0.1 et 1
        SensitivitySlider.minValue = 0.1f;
        SensitivitySlider.maxValue = 1f;

        // Utilise les PlayerPrefs pour pas que le joeuur doivent toujours remettre sa sensi comme il veut
        if (PlayerPrefs.HasKey("sensitivity"))
        {
            LoadSensitivity();
        }
        else
        {
            // fait en sorte que la sensi est à 0.5 (au centre de la barre) si ya pas de playerprefs
            PlayerPrefs.SetFloat("sensitivity", defaultSensitivity);
            LoadSensitivity();
        }
    }

    public void SetSensitivity()
    {
        float newSensitivity = SensitivitySlider.value;
        
        if (joueur != null)
        {
            joueur.sensitivityX = newSensitivity;
            joueur.sensitivityY = newSensitivity;
        }

        SaveSensitivity();
    }

    public void SaveSensitivity()
    {
        PlayerPrefs.SetFloat("sensitivity", SensitivitySlider.value);
    }

    public void LoadSensitivity()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("sensitivity", defaultSensitivity);
        SensitivitySlider.value = savedSensitivity;
        
        if (joueur != null)
        {
            joueur.sensitivityX = savedSensitivity;
            joueur.sensitivityY = savedSensitivity;
        }
    }
}
