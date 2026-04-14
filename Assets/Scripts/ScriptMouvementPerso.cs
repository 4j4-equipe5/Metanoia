using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class ScriptMouvementPerso : MonoBehaviour
{
    public Transform groundCheck;
    private bool veutSauter = false;
    public LayerMask maskSol;
    public bool grounded;
    private int friction = 5;
    private float acceleration = 20f;
    public float sensitivityX = 0.5f;
    public float sensitivityY = 0.5f;
    private float orientationX;
    private float orientationY;
    private Rigidbody joueurRb;
    public Transform cameraPivot;
    private CapsuleCollider capsuleCollider;
    private InputSystem_Actions controle;
    private float velociteMax = 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        joueurRb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        controle = new InputSystem_Actions();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Pour mettre le jeu en pause (sinon la caméra continue de bouger)
        if (PauseManager.IsPaused()){
            return;
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
    }
    private void FixedUpdate()
    {
        // Pour mettre le jeu en pause (sinon la caméra continue de bouger)
        if (PauseManager.IsPaused()) {
            return;
        }  


        //� chaque FixedUpdate, on v�rifie d'abord si le joueur est au sol
        VerifierSol();
        //ensuite, si le joueur est au sol et qu'il appui sur la touche assign�e au saut,
        if (grounded && veutSauter)
        {
            //ajouter une force au rb
            joueurRb.AddForce(Vector3.up * 2.5f, ForceMode.Impulse);
            //toggle la bool qui d�termine si la touche saut est enclanch�e
            veutSauter = false;
        }
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
    }
    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        controle.Player.Enable();
    }
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
        Debug.Log(grounded);
        return grounded;
    }


    // Permet de quitter proprement la partie
    public void DisableControls()
    {
        controle.Player.Disable();
    }
}
