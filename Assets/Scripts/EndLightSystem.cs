using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Système d'éclairage de fin du 5e round
/// Désactive tous les lumières du jeu à la fin du dernier round et crée un chemin
/// de lumières depuis le joueur jusqu'à l'ascenseur
public class EndLightSystem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float delaiAvantAllumage = 3f; // Délai avant que les lumières du chemin s'allument
    [SerializeField] private float distanceEntrePointsChemin = 15f; // Distance approximative entre les points du chemin
    [SerializeField] private float rayonDetectionElevator = 10f; // Rayon pour détecter les lumières près de l'ascenseur
    [SerializeField] private string tagElevator = "Elevator"; // Tag de l'ascenseur
    [SerializeField] private Light[] lumieresElevator; // Les 2 lumières de l'ascenseur

    [Header("Débogage")]
    [SerializeField] private bool afficherPathfinding = true;

    private Light[] toutesLesLumieres;
    private List<Light> lumieresCheminEclaire = new List<Light>();
    private bool systemActivated = false;

    void Start()
    {
        // Récupérer toutes les lumières du niveau
        toutesLesLumieres = FindObjectsOfType<Light>();
    }

    /// Déclenche la séquence de fermer et ouvrir des lumières
    /// À appeler quand le dernier round est complété
    public void ActivateEndLightSequence()
    {
        if (systemActivated) return; // Éviter les activations multiples
        
        systemActivated = true;
        StartCoroutine(ExecuteEndLightSequence());
    }

    private IEnumerator ExecuteEndLightSequence()
    {
        Debug.Log("[END LIGHT SYSTEM] Séquence d'éclairage de fin de partie activée");

        // Étape 1: Éteindre toutes les lumières immédiatement
        ExtinguishAllLights();
        
        // Étape 2: Attendre un délai
        yield return new WaitForSeconds(delaiAvantAllumage);

        // Étape 3: Calculer et allumer les lumières du chemin
        CalculateAndLightPath();
    }

    /// Éteint toutes les lumières du niveau
    private void ExtinguishAllLights()
    {
        foreach (Light light in toutesLesLumieres)
        {
            if (light != null)
            {
                light.enabled = false;
            }
        }
        Debug.Log("[END LIGHT SYSTEM] Toutes les lumières ont été éteintes");
    }

    /// Calcule le chemin et allume les lumières appropriées
    private void CalculateAndLightPath()
    {
        // Récupérer la position du joueur
        Vector3 positionJoueur = ScriptMouvementPerso.Instance.transform.position;
        
        // Trouver l'ascenseur
        Transform elevatorTransform = FindElevator();
        if (elevatorTransform == null)
        {
            Debug.LogError("[END LIGHT SYSTEM] Impossible de trouver l'ascenseur!");
            return;
        }
        
        Vector3 positionAscenseur = elevatorTransform.position;

        // Allumer les lumières de l'ascenseur
        ActivateElevatorLights();

        // Trouver les lumières qui forment un chemin
        lumieresCheminEclaire = FindPathLights(positionJoueur, positionAscenseur);

        // Allumer ces lumières
        foreach (Light light in lumieresCheminEclaire)
        {
            light.enabled = true;
        }

        Debug.Log($"[END LIGHT SYSTEM] {lumieresCheminEclaire.Count} lumières de chemin allumées");

        // Débogaage
        if (afficherPathfinding)
        {
            AfficherPathfinding(positionJoueur, positionAscenseur, lumieresCheminEclaire);
        }
    }

    //// Trouve la position de l'ascenseur
    private Transform FindElevator()
    {
        // Chercher par tag
        GameObject elevatorObj = GameObject.FindWithTag(tagElevator);
        if (elevatorObj != null)
        {
            return elevatorObj.transform;
        }

        Debug.LogWarning("[END LIGHT SYSTEM] Ascenseur non trouvé. Vérifiez le tag 'Elevator' ou les noms d'objets");
        return null;
    }

    /// Allume les 2 lumières de l'ascenseur
    private void ActivateElevatorLights()
    {
        if (lumieresElevator != null)
        {
            foreach (Light light in lumieresElevator)
            {
                if (light != null)
                {
                    light.enabled = true;
                }
            }
        }
    }

    /// Trouve les lumières qui forment un chemin du joueur à l'ascenseur
    /// Utilise des waypoints basés sur la distance
    private List<Light> FindPathLights(Vector3 start, Vector3 end)
    {
        List<Light> pathLights = new List<Light>();

        // Calculer les points de passage entre le joueur et l'ascenseur
        List<Vector3> pathPoints = GeneratePathPoints(start, end);

        // Pour chaque point de passage, trouver la lumière la plus proche
        foreach (Vector3 pathPoint in pathPoints)
        {
            Light closestLight = FindClosestLight(pathPoint, pathLights);
            if (closestLight != null)
            {
                pathLights.Add(closestLight);
            }
        }

        // Ajouter les lumières près de l'ascenseur
        List<Light> elevatorAreaLights = FindLightsNearPoint(end, rayonDetectionElevator);
        foreach (Light light in elevatorAreaLights)
        {
            if (!pathLights.Contains(light) && light != null)
            {
                pathLights.Add(light);
            }
        }

        return pathLights;
    }

    /// Génère une liste de points entre deux positions
    private List<Vector3> GeneratePathPoints(Vector3 start, Vector3 end)
    {
        List<Vector3> points = new List<Vector3> { start };
        
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int numPoints = Mathf.Max(2, Mathf.FloorToInt(distance / distanceEntrePointsChemin));

        for (int i = 1; i < numPoints; i++)
        {
            float t = (float)i / numPoints;
            Vector3 point = Vector3.Lerp(start, end, t);
            points.Add(point);
        }

        points.Add(end);
        return points;
    }

    /// Trouve la lumière la plus proche d'un point, sans répétition
    private Light FindClosestLight(Vector3 position, List<Light> alreadySelected)
    {
        Light closestLight = null;
        float closestDistance = float.MaxValue;

        foreach (Light light in toutesLesLumieres)
        {
            if (light == null || light.gameObject == null) continue;
            if (alreadySelected.Contains(light)) continue; // Éviter les répétitions
            if (!light.enabled && lumieresElevator != null && System.Array.Exists(lumieresElevator, element => element == light)) 
                continue; // Passer les lumières d'ascenseur si déjà sélectionnées

            float distance = Vector3.Distance(light.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLight = light;
            }
        }

        return closestLight;
    }

    ////// Trouuve toutes les lumières dans un rayon autour d'un point
    private List<Light> FindLightsNearPoint(Vector3 center, float radius)
    {
        List<Light> lights = new List<Light>();

        foreach (Light light in toutesLesLumieres)
        {
            if (light != null && Vector3.Distance(light.transform.position, center) <= radius)
            {
                lights.Add(light);
            }
        }

        return lights;
    }

    /// Affiche le chemin calculé (pour débogage)
    private void AfficherPathfinding(Vector3 posJoueur, Vector3 posAscenseur, List<Light> lights)
    {
        Debug.Log($"[PATHFINDING] Joueur: {posJoueur}, Ascenseur: {posAscenseur}");
        Debug.Log($"[PATHFINDING] Lumières à allumer: {lights.Count}");
        
        foreach (Light light in lights)
        {
            Debug.Log($"  - Lumière activée à {light.transform.position}");
        }
    }

    /// Réinitialise le système (pour tests)
    public void ResetSystem()
    {
        systemActivated = false;
        lumieresCheminEclaire.Clear();
        StopAllCoroutines();
    }
}
