using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class ScriptMouvementPerso : MonoBehaviour
{
    //==================================================================
    [Header("crouch")]
    
    private float hauteurCameraCrouch = 0.8f;
    private float modificateurCrouchVitesse = 0.5f;
    private float modificateurCrouchSaut = 0.5f;
    private float hauteurCrouch = 1f;
    private float hauteurCibleeCamera;
    //===================================================================
    [Header("composantsPhysiques")]
    private CapsuleCollider capsuleCollider;
     public Transform cameraPivot;
     public LayerMask maskSol;
    public Transform groundCheck;
    private Rigidbody joueurRb;
    //====================================================================
    [Header("variables de controle du personnage")]
     private bool isCrouched;
    private InputSystem_Actions controle;
    private bool veutSauter = false;
   
    public bool grounded;
    private int friction = 3;
    private float acceleration = 20f;
    public float sensitivityX = 0.5f;
    public float sensitivityY = 0.5f;
    private float orientationX;
    private float orientationY;
    private float velociteMax;
    private float velociteMaxNormale = 20f;
    private float hauteurNormale = 1.6f;
    private float jumpForce;
    private float jumpForceNormale = 2f;
    private float hauteurCameraNormale = 1f;
    //====================================================================
    [Header("References a d'autres scripts")]
    private ScriptGestionArme scriptGestionArme;
    //====================================================================
    [Header("Weapon Sway")]

    private Transform socketArme;
    private float intensiteSway = 1f;
    private float smoothnessSway = 6f;
    private Vector3 cibleRotationSway;
    //==================================================================
    //==================================================================
    //==================================================================
    
  
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        scriptGestionArme = GetComponentInChildren<ScriptGestionArme>();
        joueurRb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        controle = new InputSystem_Actions();
        jumpForce = jumpForceNormale;
        capsuleCollider.height = hauteurNormale;
        velociteMax = velociteMaxNormale;
        hauteurCibleeCamera = hauteurCameraNormale;
        socketArme = cameraPivot.Find("SocketArme");
        
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


        //obtenir les valeurs de la souris
        Vector2 look = controle.Player.Look.ReadValue<Vector2>();
        //l'axe horizontale est influenc� par le personnage
        transform.Rotate(Vector3.up * look.x * sensitivityX);

        orientationY -= look.y * sensitivityY;
        orientationY = Mathf.Clamp(orientationY, -70, 70);
        //c'est la cam�ra qui est tourn�e par l'axe verticale
        cameraPivot.localRotation = Quaternion.Euler(orientationY, 0f, 0f);
        //permet de transitionner entre crouched et debout
        cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x,
         Mathf.Lerp(cameraPivot.localPosition.y, hauteurCibleeCamera, 8f * Time.deltaTime),
            cameraPivot.localPosition.z
        );
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
        if (grounded && veutSauter)
        {
 
            //ajouter une force au rb
            joueurRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //toggle la bool qui d�termine si la touche saut est enclanch�e
            veutSauter = false;
        }
    }
    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        controle.Player.Enable();
    }

    public void DisableControls()
    {
        controle.Player.Disable();
    }

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
}
