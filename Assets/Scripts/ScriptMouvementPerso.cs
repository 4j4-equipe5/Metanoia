using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class ScriptMouvementPerso : MonoBehaviour
{
   
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
    private float velociteMax = 10f;
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
        if (controle.Player.Jump.triggered)
        {
            veutSauter = true;
        }


        //obtenir les valeurs de la souris
        Vector2 look = controle.Player.Look.ReadValue<Vector2>();
        //l'axe horizontale est influencé par le personnage
        transform.Rotate(Vector3.up * look.x * sensitivityX);

        orientationY -= look.y * sensitivityY;
        orientationY = Mathf.Clamp(orientationY, -70, 70);
        //c'est la caméra qui est tournée par l'axe verticale
        cameraPivot.localRotation = Quaternion.Euler(orientationY, 0f, 0f);
    }
    private void FixedUpdate()
    {

        VerifierSol();
        if (grounded && veutSauter)
        {
            joueurRb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
            veutSauter = false;
        }
        //obtenir la direction des touches w a s d
        Vector2 inputMove = controle.Player.Move.ReadValue<Vector2>();
        Vector3 directionAcceleration = (transform.right * inputMove.x + transform.forward * inputMove.y).normalized;
        Vector3 velociteActuelle = new Vector3(joueurRb.linearVelocity.x, 0f, joueurRb.linearVelocity.z);

        Vector3 velociteHorizontale = new Vector3(joueurRb.linearVelocity.x, 0, joueurRb.linearVelocity.z);
        Vector3 nouvelleVelocite;




        if (grounded)
        {
            nouvelleVelocite = BougerSol(directionAcceleration, velociteHorizontale);
        }
        else
        {
            nouvelleVelocite = BougerAir(directionAcceleration, velociteHorizontale);
        }
        nouvelleVelocite.y = joueurRb.linearVelocity.y;
        joueurRb.linearVelocity = nouvelleVelocite;
    }
    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        controle.Player.Enable();
    }
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
    private Vector3 BougerAir(Vector3 directionAcceleration, Vector3 velociteActuelle)
    {
        return GererAcceleration(directionAcceleration, velociteActuelle);
    }
    private bool VerifierSol()
    {
        if (Physics.CheckSphere(transform.position, 0.5f, maskSol))
        {
            grounded = true;
            Debug.Log("ground");
        }
        else
        {
            grounded = false;
            Debug.Log("air");
        }

        return grounded;
    }
}
