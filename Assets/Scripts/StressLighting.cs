using UnityEngine;

public class StressLighting : MonoBehaviour
{
    [Header("Lights")]
    public Light[] roomLights;

    [Header("Stress Colors")]
    public Color level0 = Color.white;
    public Color level1 = new Color(1f, 0.85f, 0.85f);
    public Color level2 = new Color(1f, 0.65f, 0.65f);
    public Color level3 = new Color(1f, 0.4f, 0.4f);
    public Color level4 = new Color(0.8f, 0f, 0f);

    [Header("Transition")]
    public float transitionSpeed = 2f;

    private Color targetColor;

    void Start()
    {
        targetColor = level0;
    }

    void Update()
    {
        foreach (Light lightObj in roomLights)
        {
            lightObj.color = Color.Lerp(
                lightObj.color,
                targetColor,
                transitionSpeed * Time.deltaTime
            );
        }
    }

    public void SetStressLevel(int level)
    {
        switch (level)
        {
            case 0:
                targetColor = level0;
                break;

            case 1:
                targetColor = level1;
                break;

            case 2:
                targetColor = level2;
                break;

            case 3:
                targetColor = level3;
                break;

            case 4:
                targetColor = level4;
                break;
        }
    }
}