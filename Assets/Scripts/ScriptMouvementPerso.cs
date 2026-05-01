using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class ScriptMouvementPerso : MonoBehaviour, IDommagable
{
    //==================================================================
    // SECTION ACCROUPISSEMENT (CROUCH)
    //==================================================================
    [Header("crouch")]
    
    private float hauteurCameraCrouch = 0.8f; // hauteur de la caméra quand le joueur est accroupi
    private float modificateurCrouchVitesse = 0.5f; // multiplicateur de vitesse lors de l'accroupissement (50% de la vitesse normale)
    private float modificateurCrouchSaut = 0.5f; // multiplicateur de force de saut lors de l'accroupissement (50% de la force normale)
    private float hauteurCrouch = 1f; // hauteur du collider du joueur en position accroupie
    private float hauteurCibleeCamera; // hauteur cible de la caméra (interpolée entre normale et accroupie)
    //===================================================================
    // SECTION COMPOSANTS PHYSIQUES
    //===================================================================
    [Header("composantsPhysiques")]
    private CapsuleCollider capsuleCollider; // collider capsule du joueur pour détecter les collisions
    public Transform cameraPivot; // point pivot autour duquel tourne la caméra
    public LayerMask maskSol; // masque de couche pour identifier le sol
    public Transform groundCheck; // point de vérification de contact avec le sol
    private Rigidbody joueurRb; // composant Rigidbody pour la physique du joueur
    private Camera camJoueur;
    //====================================================================
    // SECTION VARIABLES DE CONTRÔLE DU PERSONNAGE
    //====================================================================
    [Header("variables de controle du personnage")]
    private bool isCrouched; // indicateur si le joueur est actuellement accroupi
    private InputSystem_Actions controle; // système d'entrée pour gérer les contrôles
    private bool veutSauter = false; // flag indiquant un saut demandé par le joueur
   
    public bool grounded; // indicateur si le joueur est au sol
    private int friction = 3; // coefficient de friction au sol (décelération)
    private float acceleration = 20f; // accélération du joueur
    public float sensitivityX = 0.5f; // sensibilité horizontale (rotation du corps)
    public float sensitivityY = 0.5f; // sensibilité verticale (rotation de la caméra)
    private float orientationX; // rotation horizontale du corps (inutilisée pour le moment)
    private float orientationY; // rotation verticale de la caméra (axe X)
    private float velociteMax; // vitesse maximale actuelle du joueur
    private float velociteMaxNormale = 50f; // vitesse maximale en position normale
    private float hauteurNormale = 1.6f; // hauteur du collider du joueur en position normale
    private float jumpForce; // force de saut actuelle
    private float jumpForceNormale = 3f; // force de saut en position normale
    private float hauteurCameraNormale = 1.6f; // hauteur de la caméra en position normale
    private float cooldownSaut = 0.4f;
    private float dernierSaut = 0f;
    private float porteInteraction = 3f;
    public LayerMask masqueInteraction;

    private IInteraction cibleActuelle;
    //====================================================================
    // SECTION RÉFÉRENCES À D'AUTRES SCRIPTS
    //====================================================================
    [Header("References a d'autres scripts")]
    private ScriptGestionArme scriptGestionArme; // référence au script de gestion des armes

    private ScriptGestionArme[] slotsArmes = new ScriptGestionArme[2];
    private int indexArmeActive = 0;
   
    //====================================================================
    // SECTION WEAPON SWAY (BALANCEMENT DE L'ARME)
    //====================================================================
    [Header("Weapon Sway")]
    
    private Transform socketArme; // point d'attache de l'arme (socket)
    private float intensiteSway = 1f; // intensité du balancement de l'arme
    private float smoothnessSway = 6f; // fluidité du balancement de l'arme
    private Vector3 cibleRotationSway; // rotation cible du balancement
    //==================================================================
    // SECTION STATS ET UI
    //==================================================================
    [Header("Stats joueur")]
    [SerializeField] private int hpPlayer = 100; // points de vie du joueur
    [SerializeField] public GameObject gameOverScreen; // écran de game over à afficher à la mort
    private bool firstFrame = true; // flag pour éviter la transition lerp au démarrage
    //==================================================================
    //==================================================================
    //===================================================================
    
  
   
    //==================================================================
    // INITIALISATION - APPELÉE UNE FOIS AVANT LE PREMIER FRAME
    //==================================================================
    private void Awake()
    {

        // Récupérer le composant ScriptGestionArme dans les enfants de ce GameObject
        InitArmes();
        // Récupérer le composant Rigidbody pour appliquer les forces
        joueurRb = GetComponent<Rigidbody>();
        // Récupérer le composant CapsuleCollider pour modifier la taille lors de l'accroupissement
        capsuleCollider = GetComponent<CapsuleCollider>();
        // Initialiser le système d'entrée
        controle = new InputSystem_Actions();
        // Définir les valeurs initiales (position normale)
        jumpForce = jumpForceNormale;
        capsuleCollider.height = hauteurNormale;
        capsuleCollider.center = new Vector3(0, hauteurNormale / 2f, 0);
        velociteMax = velociteMaxNormale;
        hauteurCibleeCamera = hauteurCameraNormale;
        // Initialiser la caméra à la bonne hauteur pour éviter le pop au démarrage
        cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x, hauteurCameraNormale, cameraPivot.localPosition.z);
        // Trouver le socket d'arme pour appliquer le balancement (sway)
        socketArme = cameraPivot.Find("SocketArme");

        camJoueur = Camera.main;
        
    }

    //==================================================================
    //==================================================================
    //==================================================================

    void Start()
    {
       
    }

    //==================================================================
    //==================================================================
    //==================================================================

    // Update is called once per frame
    void Update()
    {
        AppliquerSway();

        if (controle.Player.Attack.IsPressed())
        {
            scriptGestionArme.Tirer();
        }
        if (controle.Player.Crouch.triggered)
        {
            VerifierCrouch(!isCrouched);
        }
        if (controle.Player.Jump.triggered)
        {
            veutSauter = true;
        }
        if (controle.Player.Previous.triggered) ChangerArme(0);
        if(controle.Player.Next.triggered) ChangerArme(1);


        //obtenir les valeurs de la souris
        Vector2 look = controle.Player.Look.ReadValue<Vector2>();
        //l'axe horizontale est influenc� par le personnage
        transform.Rotate(Vector3.up * look.x * sensitivityX);

        orientationY -= look.y * sensitivityY;
        orientationY = Mathf.Clamp(orientationY, -70, 70);
        //c'est la cam�ra qui est tourn�e par l'axe verticale
        cameraPivot.localRotation = Quaternion.Euler(orientationY, 0f, 0f);
        //permet de transitionner entre crouched et debout
        if (firstFrame)
        {
            cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x, hauteurCibleeCamera, cameraPivot.localPosition.z);
            firstFrame = false;
        }
        else
        {
            cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x,
             Mathf.Lerp(cameraPivot.localPosition.y, hauteurCibleeCamera, 8f * Time.deltaTime),
                cameraPivot.localPosition.z
            );
        }
    }
    

    //==================================================================
    //==================================================================
    //==================================================================

    private void FixedUpdate()
    {

        //obtenir la direction des touches w a s d
        Vector2 inputMove = controle.Player.Move.ReadValue<Vector2>();
        Vector3 directionAcceleration = (transform.right * inputMove.x + transform.forward * inputMove.y).normalized;
        //assigner velociteActuelle au linearvelocity du rb du joueur avant les modifications
        Vector3 velociteActuelle = new Vector3(joueurRb.linearVelocity.x, 0f, joueurRb.linearVelocity.z);

        Vector3 velociteHorizontale = new Vector3(joueurRb.linearVelocity.x, 0, joueurRb.linearVelocity.z);
        Vector3 nouvelleVelocite;



        //si le joueur est a terre, on calcule la velocite avec la fonction BougerSol, sinon avec BougerAir
        if (grounded)
        {
            nouvelleVelocite = BougerSol(directionAcceleration, velociteHorizontale);
        }
        else
        {
            nouvelleVelocite = BougerAir(directionAcceleration, velociteHorizontale);
        }
        //le y de la velocite reste inchang�
        nouvelleVelocite.y = joueurRb.linearVelocity.y;
        //on applique la nouvellevelocite au rb
        joueurRb.linearVelocity = nouvelleVelocite;
                //� chaque FixedUpdate, on v�rifie d'abord si le joueur est au sol
        VerifierSol();
        //ensuite, si le joueur est au sol et qu'il appui sur la touche assign�e au saut,
        if (grounded && veutSauter && Time.time > dernierSaut+cooldownSaut)
        {
 
            //ajouter une force au rb
            joueurRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //toggle la bool qui d�termine si la touche saut est enclanch�e
            veutSauter = false;
            dernierSaut = Time.time;
        }
    }
    //==================================================================
    // ÉVÉNEMENTS D'ACTIVATION
    //==================================================================
    private void OnEnable()
    {
        // Verrouiller le curseur au démarrage (mode jeu FPS)
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        // Activer le système d'entrée
        controle.Player.Enable();
    }

    //==================================================================
    // DÉSACTIVATION DES CONTRÔLES
    //==================================================================
    /// <summary>
    /// Désactive les contrôles du joueur (pour les menus, dialogues, etc.)
    /// </summary>
    public void DisableControls()
    {
        controle.Player.Disable();
    }

    /// <summary>
    /// Désactive complètement le mouvement du joueur (utilisé en cas de victoire, mort, etc.)
    /// </summary>
    public void DisableMovement()
    {
        controle.Player.Disable();
        this.enabled = false; // Désactiver le script complet pour arrêter tous les calculs
    }

    /// <summary>
    /// Réactive les contrôles du joueur
    /// </summary>
    public void EnableControls()
    {
        controle.Player.Enable();
    }
    
    
    
    //==================================================================
    //==================================================================
    //==================================================================
    
    /// <summary>
    /// Fonction qui sera utilis�e dans les fonctions bougerSol et BougerAir. Dans bougerAir, elle est utilis�e tel quel, alors que dans BougerSol, on ajoute une r�duction de vitesse en fonction du temps et dela friction du sol.
    /// </summary>
    /// <param name="directionAcceleration">Valeur calcul�e dans le fixed update. Valeur normalis�e qui correspond a la direction du rb du joueur</param>
    /// <param name="velociteActuelle">dans le fixedupdate, cette valeur est assign�e avant le calcul de la nouvellevelocite et correspond au linearvelocity du rb du joueur</param>
    /// <returns>velociteActuelle + directionAcceleration * velociteAccel</returns>
    private Vector3 GererAcceleration(Vector3 directionAcceleration, Vector3 velociteActuelle)
    {
        float velociteFuture = Vector3.Dot(velociteActuelle, directionAcceleration);
        float velociteAccel = acceleration * Time.fixedDeltaTime;
        if (velociteFuture + velociteAccel > velociteMax)
        {
            velociteAccel = velociteMax - velociteFuture;
        }
        return velociteActuelle + directionAcceleration * velociteAccel;
    }

    //==================================================================
    //==================================================================
    //==================================================================


    /// <summary>
    /// calcul une r�duction de la velocite avant de retourner GererAcceleration pour quand le personnage est au sol. Appel�e dans FixedUpdate pour assigner une valeur � nouvelleVelocit�, qui sera ensuite la valeur de joueurRb.linearVelocity
    /// </summary>
    /// <param name="directionAcceleration">idem a GererAcceleration</param>
    /// <param name="velociteActuelle">idem GererAcceleration</param>
    /// <returns>GererAcceleration(directionAcceleration, velociteActuelle)</returns>
    private Vector3 BougerSol(Vector3 directionAcceleration, Vector3 velociteActuelle)
    {
        float vitesse = velociteActuelle.magnitude;
        bool inputPresent = directionAcceleration.magnitude > 0.01f;
        if (inputPresent && vitesse > 0.01f)
        {
            float reduction = vitesse * friction * Time.fixedDeltaTime;
            float nouvelleVitesse = Mathf.Max(vitesse - reduction, 0);
            velociteActuelle *= nouvelleVitesse / vitesse;
        }
        return GererAcceleration(directionAcceleration, velociteActuelle);
    }

    //==================================================================
    //==================================================================
    //==================================================================

    /// <summary>
    /// retourne GererAcceleration sans la r�duction de BougerSol
    /// </summary>
    /// <param name="directionAcceleration">idem aux autres fonctions</param>
    /// <param name="velociteActuelle">idem</param>
    /// <returns>GererAcceleration(directionAcceleration, velociteActuelle);</returns>
    private Vector3 BougerAir(Vector3 directionAcceleration, Vector3 velociteActuelle)
    {
        return GererAcceleration(directionAcceleration, velociteActuelle);
    }
    /// <summary>
    /// fonction qui retourne un bool qui d�termine si le joueur est au sol
    /// </summary>
    /// <returns>grounded</returns>
    private bool VerifierSol()
    {
        grounded = Physics.CheckSphere(groundCheck.position, 0.2f, maskSol);
       
        return grounded;
    }
    /// <summary>
    /// fonction qui gere les modification des stats quand le joueur est
    /// accroupi
    /// </summary>
    /// <param name="state">bool que l'on passe pour gerer le comportement</param>
    private void VerifierCrouch(bool state)
    {
        isCrouched = state;
        if (isCrouched)
        {
            velociteMax = velociteMaxNormale * modificateurCrouchVitesse;
            jumpForce = jumpForceNormale * modificateurCrouchSaut;
            capsuleCollider.center = new Vector3(0, hauteurCrouch / 2f, 0);
            capsuleCollider.height = hauteurCrouch;

            hauteurCibleeCamera = hauteurCameraCrouch;
          
             
        }
        else
        {
            velociteMax = velociteMaxNormale;
             capsuleCollider.center = new Vector3(0, hauteurNormale / 2f, 0);
            capsuleCollider.height = hauteurNormale;
            jumpForce = jumpForceNormale;
            hauteurCibleeCamera = hauteurCameraNormale;
        }
        
    }
    /// <summary>
    /// fonction qui calcule et applique le sway du socket d'arme en fonction
    /// du mouvement de la camera. Appelee dans update
    /// </summary>
    private void AppliquerSway()
    {
        Vector2 look = controle.Player.Look.ReadValue<Vector2>();

        cibleRotationSway = new Vector3(look.y * intensiteSway,
        look.x * intensiteSway, 0f
        );

        socketArme.localRotation = Quaternion.Lerp(
        socketArme.localRotation,
        Quaternion.Euler(cibleRotationSway),
        Time.deltaTime * smoothnessSway);
    }
    // implémentation de IDommagable pour recevoir des dégâts
    public void PrendreDegat(int degats)
    {
        hpPlayer -= degats;
        Debug.Log($"Player received {degats} dmg. HP = {hpPlayer}");
        if (hpPlayer <= 0)
        {
            // simple gestion de la mort (à adapter)
            Debug.Log("Player dead");
            // par exemple désactiver le GameObject
            gameObject.SetActive(false);
            // afficher l'écran de game over
            gameOverScreen.SetActive(true);
            
        }
    }
/// <summary>
/// fonction qui va chercher les deux armes dans l'inventaire du joueur dans awake
/// </summary>
    private void InitArmes()
    {
        ScriptGestionArme[] armesPresentes = GetComponentsInChildren<ScriptGestionArme>(true);
        foreach (ScriptGestionArme arme in armesPresentes)
        {
            int slot = arme.slotIndex;
            if(slot < slotsArmes.Length)
            {
                slotsArmes[slot] = arme;
                arme.gameObject.SetActive(slot == indexArmeActive);
            }
        }
        scriptGestionArme = slotsArmes[indexArmeActive];
    }
    /// <summary>
    /// fonction qui gere le changement d'arme
    /// </summary>
    /// <param name="index"></param>
    private void ChangerArme (int index)
    {
        if (slotsArmes[index] == null) return;
        if (index == indexArmeActive) return;

        slotsArmes[indexArmeActive].gameObject.SetActive(false);
        indexArmeActive = index;
        slotsArmes[indexArmeActive].gameObject.SetActive(true);
        scriptGestionArme = slotsArmes[indexArmeActive];
    }
    /// <summary>
    /// fonction appelee dans update, qui utilise un raycast pour detecter les objets
    /// avec lesquels le joueur peut interagir.
    /// </summary>
    void detecterInteraction()
    {
        Ray ray = new Ray(camJoueur.transform.position,camJoueur.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit,porteInteraction,masqueInteraction))
        {
            var objetInteractif = hit.collider.GetComponent<IInteraction>();

            if(objetInteractif != null)
            {
                cibleActuelle = objetInteractif;
                return;
            }
            
        }
        cibleActuelle = null;
    }

}
