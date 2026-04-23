using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.Serialization;
public class ControleMiann : MonoBehaviour, IDommagable
{
    [Header("Références aux objets")]
    [SerializeField] public NavMeshAgent agentMiann;
    [SerializeField] public Transform cibleMiann; // la position de la cible de Miann peut-être définie dans l'inspecteur 
    [SerializeField] public int distanceDetection = 10; // distance à laquelle Miann détecte le joueur
    [SerializeField] public Transform[] waypoints; // les points de patrouille de Miann
    [SerializeField] public GameObject endGameScreen; // écran de fin de jeu à afficher lorsque le joueur est tué par Miann
    [SerializeField] public GameObject menuVictoire; // référence au menu de victoire pour l'afficher lorsque le joueur gagne
    private int currentWaypointIndex = 0; // index du point de patrouille actuel    
    // ==================================================================================================
    [Header("Paramètres de Miann")]
    [SerializeField] public float distanceAttaque = 3.0f; // distance à laquelle Miann attaque le joueur
    [SerializeField] public float distancePoursuite = 5f; // distance à laquelle Miann poursuit le joueur
    [SerializeField] private static int pointVictoireDemo = 0;
    private bool isStunned = false; // indique si Miann est étourdi (stunned) après avoir pris des dégâts
    private int etatMiann = 0; // 0 = patrouille, 1 = poursuite, 2 = attaque, monte mur
    private int hpMiann = 100; // points de vie de Miann
    private int degatsMiann = 10; // dégâts infligés par Miann
    private float tempsAttaque = 5f; // temps entre les attaques de Miann
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
    //===================================================================================================
    public void PrendreDegat(int degats) // fonction appelée quand Miann prend des dégâts
    {
        hpMiann -= degats; // transfer de int (dégats) de ScriptGestionArme => IDommagable => ControleMiann 
        isStunned = true; // marque Miann comme étourdi après avoir pris des dégâts
        StartCoroutine(Stunned()); // lance la coroutine stun qui gère la durée du stun
        if (hpMiann <= 0) // si Miann à zero Pv il meurt
        {
            agentMiann.isStopped = true; // arrête le NavMeshAgent avant destruction
            Destroy(gameObject); // détruit le gameobject de Miann
            pointVictoireDemo ++;
            VictoryManager menuVictoire = FindFirstObjectByType<VictoryManager>(); // trouve le script de gestion du menu de victoire pour l'afficher
            menuVictoire.AjouterMort();  
        }
    }
    IEnumerator Stunned() // coroutine pour gérer le stun de Miann après avoir pris des dégâts
    {
        agentMiann.isStopped = true; // arrête le mouvement de Miann pendant le stun
        yield return new WaitForSeconds(3f); // durée du stun (3 secondes)
        agentMiann.isStopped = false; // Miann reprend son mouvement après le stun
        isStunned = false; // marque Miann comme n'étant plus étourdi
    }
    // ==================================================================================================
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

        if (cibleMiann == null || agentMiann == null || isStunned) return; // si aucune cible ou agent assigné ou si Miann est stun, sort de la fonction

        float dist = Vector3.Distance(transform.position, cibleMiann.position); // la distabce de  Miann à sa cible
        
        if (dist <= distanceAttaque) // si la distance est plus petit que la distance attaque
        {
            // attaque 
            etatMiann = 2; // change l'état à mode attaque
            // agentMiann.SetDestination(transform.position); // Miann arrête de bouger en se fixant à sa position actuelle
            agentMiann.isStopped = true; // arrête le NavMeshAgent pour que Miann ne bouge pas pendant l'attaque
            Debug.Log("Miann attaque !");
            timerAttaque += Time.deltaTime; // le cooldown
            StartCoroutine(Attaque());
        }
        else if (dist <= distancePoursuite || dist <= distanceDetection) // si la distance est plus petit que la distance de poursuite ou de detection
        {
            // poursuite
            etatMiann = 1;
            agentMiann.isStopped = false;
            agentMiann.SetDestination(cibleMiann.position);
        }
        else // si le joueur est hors de portée, Miann patrouille entre les points de patrouille
        {
            // patrouille
            etatMiann = 0;
            Patrol();
        }
        if (pointVictoireDemo >= 3)
        {
           VictoryManager menuVictoire = FindFirstObjectByType<VictoryManager>(); // trouve le script de gestion du menu de victoire pour l'afficher
        }
    }
    // Coroutine pour gérer l'attaque de Miann avec un cooldown entre les attaques et pour infliger des dégâts à la cible si elle implémente l'interface IDommagable
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
        agentMiann.SetDestination(cibleMiann.position); // Miann reprend sa poursuite après l'attaque
    }
    //===================================================================================================
    private void Patrol() // fonction pour faire patrouiller Miann entre les points de patrouille
    {
        if (waypoints == null || waypoints.Length == 0) return; // si aucun point de patrouille assigné, sort de la fonction pour éviter les erreurs
        
        agentMiann.isStopped = false; // assure que Miann n'est pas arrêté
        agentMiann.SetDestination(waypoints[currentWaypointIndex].position); // Mian se dirige vers le point de patrouille actuel d'une liste de points de patrouille définis dans l'inspecteur
         
        if (!agentMiann.pathPending && agentMiann.remainingDistance <= agentMiann.stoppingDistance + 0.1f) // si Miann est proche du point de patrouille actuel, passe au point suivant
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
