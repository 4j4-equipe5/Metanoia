using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
public class ScriptGestionArme : MonoBehaviour
{
    [SerializeField] private Transform portEjection;
    private GameObject flashActuel;
    public bool peutRecevoirInput = true;
    private ScriptMouvementPerso joueur;

    private Vector3 positionInitialeArme;
    private float vitesseRetourRecul = 10f;
    public int slotIndex = 0;
    
    private Transform socketArme;
    private float dernierTir;
    private float tempsEntreCoup;
    public dataArmes donnees;
    [SerializeField] public AudioSource sonTir;
    public int reserveMunitionActuelle;
    private Camera cam;
    public bool estObtenue = false;
   
    [SerializeField] private Transform pointMuzzleFlash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        joueur = joueur = GetComponentInParent<ScriptMouvementPerso>();
        positionInitialeArme = transform.localPosition;
        socketArme = transform.parent;
        tempsEntreCoup = 1f/donnees.cadence;
        reserveMunitionActuelle = donnees.maxReserve;
        cam = Camera.main;
        
        
        
    }
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// fonction qui retourne un bool en fonction de si assez de temps
    /// s'est ecoule depuis le dernier coup et si il reste des munitions
    /// </summary>
    /// <returns></returns>
    public bool PeutTirer()
    {
        return peutRecevoirInput && Time.time >= dernierTir+ tempsEntreCoup && reserveMunitionActuelle > 0;
    }
    /// <summary>
    /// fonction qui enregistre le temps au moment du dernier tir
    /// </summary>
    public void EnregistrerTir()
    {
        dernierTir = Time.time;
    }
    /// <summary>
    /// fonction principale du fonctionnement de l'arme. Appelee dans
    /// le script de gestion du personnage
    /// </summary>
    public void Tirer()
    {
        //si l'arme n'est pas prete a tirer, sort de la fonction
        if (!PeutTirer())
        {
            return;
        }

        //appel de la fonction qui enregistre le temps du tir
        EnregistrerTir();
        //appel de la fonction qui ejecte une cartouche
        EjecterCartouche();
        //coroutine du muzzleflash
        StartCoroutine(MuzzleFlash());
        //applique l'effet visuel de recul sur l'arme, pas le recul de la cam
       AppliquerRecul();
       //applique le recul de la camera
       joueur.AppliquerOffsetReculCamera(joueur.recupRecul,donnees.reculParTir);
        
        // son de tir
        sonTir.PlayOneShot(sonTir.clip);
        //on retire une cartouche de la reserve
        --reserveMunitionActuelle;
        // 1. On crée le masque : on récupère l'index du layer "Player"
        // Le signe '~' inverse le masque (il veut dire "Tout sauf ça")
        int layerJoueur = LayerMask.NameToLayer("Player");
        int masqueTir = ~(1 << layerJoueur);
        //on cast le ray en fonction du nombre de projectile par coup, cela permet de donner un effet chevrotine au shotgun
        for(int i = 0; i<donnees.nombreProjectile; i++)
        {
              Vector3 direction = cam.transform.forward;
            if (donnees.nombreProjectile > 1)
            {
              //si plusieur projectiles par coup (chevrotine), dispersion aleatoire
                direction += new Vector3(Random.Range(-donnees.dispersion, donnees.dispersion),
                                    Random.Range(-donnees.dispersion, donnees.dispersion),
                                    0f
            );
            direction.Normalize(); 
            }
            Ray ray = new Ray(cam.transform.position, direction);
        //variable qui contient l'info des choses touchees
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, masqueTir)) 
            {
                if (donnees.prefabImpact != null) 
                {
                    GameObject impact = Instantiate(donnees.prefabImpact, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 1f);
                }
                //on obtient le comportement propre a l'objet pour recevoir
                //des dommages en allant chercher l'interface qu'il herite
                //pour recevoir des dommages
                IDommagable cible = hit.collider.GetComponent<IDommagable>();
                if(cible != null)
                {
                    cible.PrendreDegat((int)(donnees.degats * joueur.modificateurDommageGlobal));
                    

                }

            }
        }
        //cast un rayon en avant a partir de la camera fps
      
    }
    private void AppliquerRecul()
    {
       transform.localPosition = positionInitialeArme + Vector3.back*donnees.intensiteRecul;
        StartCoroutine(RetourPositionArme());

    }
/// <summary>
/// Co-routine qui genere un muzzleflash dans tir()
/// </summary>
/// <returns></returns>
    private IEnumerator MuzzleFlash()
    {
        if(donnees.prefabMuzzleFlash == null)
        {
            yield break;
        }
        flashActuel = Instantiate(donnees.prefabMuzzleFlash, pointMuzzleFlash.position, pointMuzzleFlash.rotation);

        float rotationAleatoire = Random.Range(0f, 360f);
        flashActuel.transform.Rotate(0f, 0f, rotationAleatoire);

        float tailleAleatoire = Random.Range(0.5f, 0.8f);
        flashActuel.transform.localScale = Vector3.one * tailleAleatoire;

        yield return new WaitForSeconds(0.025f);

        Destroy(flashActuel);
        flashActuel = null;

    }
    /// <summary>
    /// fonction qui retire les flash de la map. Elle doit etre appelee pour eviter les bugs ou le flash reste instantie si le joueur change d'arme trop vite
    /// </summary>
    public void NettoyerFlash()
    {
        if(flashActuel != null)
        {
            Destroy(flashActuel);
            flashActuel = null;
        }
    }
    /// <summary>
    /// coroutine qui gere le retour de l'arme a sa position normale avec un Lerp apres le recul du coup
    /// </summary>
    /// <returns></returns>
    private IEnumerator RetourPositionArme()
    {
        while (Vector3.Distance(transform.localPosition, positionInitialeArme)> 0.001f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                positionInitialeArme,
                Time.deltaTime * vitesseRetourRecul
            ); 
            yield return null;
        }
        transform.localPosition = positionInitialeArme;
        
    }
    /// <summary>
    /// fonction pour l'instantiation des cartouches vides. 
    /// </summary>
    private void EjecterCartouche()
    {
       if(donnees.prefabCartouche == null || portEjection == null) return; 
        GameObject cartouche = Instantiate(
            donnees.prefabCartouche,
            portEjection.position,
            portEjection.rotation
        );

       Rigidbody rbCartouche = cartouche.GetComponent<Rigidbody>();
       if(rbCartouche != null)
        {
        Vector3 forceEjection = cam.transform.right * 10f  
                                    + cam.transform.up * 10f;      
        rbCartouche.AddForce(forceEjection, ForceMode.Impulse);
        rbCartouche.AddTorque(Random.insideUnitSphere * 150f, ForceMode.Impulse);
        }
        Destroy(cartouche, 2f);
    }

  
}
