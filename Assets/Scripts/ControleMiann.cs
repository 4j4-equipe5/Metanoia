using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class ControleMiann : MonoBehaviour, IDommagable
{
    [Header("Références aux objets")]
    [SerializeField] public NavMeshAgent agentMiann;
    [SerializeField] public Transform cibleMiann; // la position de la cible de Miann peut-être définie dans l'inspecteur 
    [SerializeField] public int distanceDetection = 10; // distance à laquelle Miann détecte le joueur
    [SerializeField] public Transform[] waypoints; // les points de patrouille de Miann
    private int currentWaypointIndex = 0; // index du point de patrouille actuel    
    // ==================================================================================================
    [Header("Paramètres de Miann")]
    [SerializeField] public float distanceAttaque = 2f; // distance à laquelle Miann attaque le joueur
    [SerializeField] public float distancePoursuite = 5f; // distance à laquelle Miann poursuit le joueur
    private int etatMiann = 0; // 0 = patrouille, 1 = poursuite, 2 = attaque, monte mur
    private int hpMiann = 100; // points de vie de Miann
    private int degatsMiann = 10; // dégâts infligés par Miann
    private float tempsAttaque = 1f; // temps entre les attaques de Miann
    private float timerAttaque = 0f; // timer pour gérer le temps entre les attaques de Miann
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // null pour éviter les erreurs si on oublie d'assigner les références dans l'inspecteur
        if (agentMiann == null) agentMiann = GetComponent<NavMeshAgent>();
        // si aucune cible assignée dans l'inspecteur, cherche l'objet tagué "Player"
        // cela permet de s'assurer que Miann a une cible même si on oublie de l'assigner dans l'inspecteur

        if (cibleMiann == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player"); // cherche un objet avec le tag "Player"
            if (playerObj != null) cibleMiann = playerObj.transform; // assigne la cible à la position du joueur
        }
        if (agentMiann != null) 
        {
            agentMiann.stoppingDistance = distanceAttaque;
        }
    }

    
    public void PrendreDegat(int degats)
    {
        hpMiann -= degats; // transfer de int (dégats) de ScriptGestionArme => IDommagable => ControleMiann 
        if (hpMiann <= 0) // Si Miann à zero Pv il meurt
        {
            // Code pour gérer la mort de Miann
            Destroy(gameObject);
        }
    }
    // Update is called oonce per frame
    /*** À chaque Frame le Monstre nommer Miann va chercher si ca cible Le joueur est dans sa zone de detection, si oui il va poursuivre le joueur,
    si le joueur est dans sa zone d'attaque il va attaquer le joueur, sinon il patrouille entre les points de patrouille définis dans l'inspecteur
    **/
    void Update() 
    {
        if (cibleMiann == null) // si aucune cible assignée dans l'inspecteur, cherche l'objet tagué "Player"
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player"); // cherche un objet avec le tag "Player"
            if (playerObj != null) cibleMiann = playerObj.transform; // assigne la cible à la position du joueur
        }

        if (cibleMiann == null || agentMiann == null) return; // si aucune cible ou agent assigné, sort de la fonction pour éviter les erreurs

        float dist = Vector3.Distance(transform.position, cibleMiann.position); // la distabce de  Miann à sa cible
        
        if (dist <= distanceAttaque) // si la distance est plus petit que la distance attaque
        {
            // attaque 
            etatMiann = 2; // change l'état à mode attaque
            agentMiann.isStopped = true; // Miann arrête de bouger
            timerAttaque += Time.deltaTime; // le cooldown
            StartCoroutine(Attaque());
        }
        else if (dist <= distancePoursuite || dist <= distanceDetection)
        {
            // poursuite
            etatMiann = 1;
            agentMiann.isStopped = false;
            agentMiann.SetDestination(cibleMiann.position);
        }
        else
        {
            // patrouille
            etatMiann = 0;
            Patrol();
        }
    }
    IEnumerator Attaque()
    {
        if (timerAttaque >= tempsAttaque) 
        {
            timerAttaque = 0f;
            // infliger des dégâts si la cible implémente IDommagable
            IDommagable cible = cibleMiann.GetComponent<IDommagable>();
            if (cible != null)
            {
                cible.PrendreDegat(degatsMiann); // inflige les dégâts à la cible
            }
        }
        yield return new WaitForSeconds(tempsAttaque); // attend la prochaine frame avant de continuer l'exécution
        etatMiann = 1; // après l'attaque, Miann retourne en mode poursuite pour continuer à suivre le joueur
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (agentMiann.isStopped) agentMiann.isStopped = false;
        Transform wp = waypoints[currentWaypointIndex];
        agentMiann.SetDestination(wp.position);
        if (!agentMiann.pathPending && agentMiann.remainingDistance <= agentMiann.stoppingDistance + 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
