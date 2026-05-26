using UnityEngine;
using System.Collections;

public class MenuCameraMouvement : MonoBehaviour
{
    [Header("Movement")]
    public float startZ = -20f;
    public float endZ = 22f;
    public float speed = 0.5f;

    [Header("Fade")]
    public ScreenFade screenFade;
    public float fadeTriggerOffset = 2f; // start fading before end

    private Vector3 startPosition;
    private bool isTransitioning = false;

    void Start()
    {
        startPosition = new Vector3(1f, 1f, startZ);
        transform.position = startPosition;
    }

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;

        if (!isTransitioning && transform.position.z >= endZ - fadeTriggerOffset)
        {
            StartCoroutine(LoopTransition());
        }
    }

    IEnumerator LoopTransition()
    {
        isTransitioning = true;

        // Start fade OUT while still moving
        yield return screenFade.StartCoroutine(screenFade.FadeOut());

        // Instantly reset position while screen is black
        transform.position = startPosition;

        // Fade IN immediately
        yield return screenFade.StartCoroutine(screenFade.FadeIn());

        isTransitioning = false;
    }
}