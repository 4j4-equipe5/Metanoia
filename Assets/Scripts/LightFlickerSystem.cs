using UnityEngine;
using System.Collections;

public class LightFlickerSystem : MonoBehaviour
{
    [System.Serializable]
    public class LightGroup
    {
        public Light[] lights;
    }

    [Header("Light Groups")]
    public LightGroup[] lightGroups;

    [Header("Timing")]
    public float minDelay = 0.05f;
    public float maxDelay = 0.4f;

    void Start()
    {
        StartCoroutine(FlickerLoop());
    }

    IEnumerator FlickerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            // Pick one GROUP
            LightGroup group = lightGroups[Random.Range(0, lightGroups.Length)];

            // Flicker each light inside the group (advanced effect)
            foreach (Light l in group.lights)
            {
                if (l == null) continue;

                StartCoroutine(FlickSingleLight(l));
            }
        }
    }

    IEnumerator FlickSingleLight(Light l)
    {
        float originalIntensity = l.intensity;

        // quick flicker burst
        l.enabled = false;
        yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));

        l.enabled = true;
        l.intensity = Random.Range(0.3f, originalIntensity);

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        // sometimes fully recover, sometimes stay unstable
        if (Random.value > 0.3f)
        {
            l.intensity = originalIntensity;
        }
    }
}