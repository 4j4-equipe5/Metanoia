using UnityEngine;
using System.Collections;
public class ScriptGestionArme : MonoBehaviour
{
    private Transform pointMuzzleFlash;  
    private Transform socketArme;
    private float dernierTir;
    private float tempsEntreCoup;
    public dataArmes donnees;
    private int reserveMunitionActuelle;
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        socketArme = transform.parent;
        tempsEntreCoup = 1f/donnees.cadence;
        reserveMunitionActuelle = donnees.maxReserve;
        cam = Camera.main;
        pointMuzzleFlash = transform.Find("pointMuzzleFlash");
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
    /// c'est ecoule depuis le dernier coup et si il reste des munitions
    /// </summary>
    /// <returns></returns>
    public bool PeutTirer()
    {
        return Time.time >= dernierTir+ tempsEntreCoup && reserveMunitionActuelle > 0;
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
        StartCoroutine(MuzzleFlash());
        Debug.Log("tire");

        //on retire une cartouche de la reserve
        --reserveMunitionActuelle;

        //cast un rayon en avant a partir de la camera fps
        Ray ray = new Ray(cam.transform.position,cam.transform.forward);

        //variable qui contient l'info des choses touchees
        RaycastHit hit;

         if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
        {
            //on obtient le comportement propre a l'objet pour recevoir
            //des dommages en allant chercher l'interface qu'il herite
            //pour recevoir des dommages
            IDommagable cible = hit.collider.GetComponent<IDommagable>();
            if(cible != null)
            {
                cible.PrendreDegat(donnees.degats);

            }

        }
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
        GameObject flash = Instantiate(donnees.prefabMuzzleFlash, pointMuzzleFlash.position, pointMuzzleFlash.rotation);

        float rotationAleatoire = Random.Range(0f, 360f);
        flash.transform.Rotate(0f, 0f, rotationAleatoire);

        float tailleAleatoire = Random.Range(0.5f, 0.8f);
        flash.transform.localScale = Vector3.one * tailleAleatoire;

        yield return new WaitForSeconds(0.01f);

        Destroy(flash);

    }
  
}
