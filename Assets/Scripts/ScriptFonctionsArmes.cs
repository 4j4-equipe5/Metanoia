using UnityEngine;

public class ScriptFonctionsArmes : MonoBehaviour
{
    /*
    public dataArmes data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 public void essaiTir()
    {
        

        if (Time.time < prochainTir)
        {
            return;
        }
        if (isReloading) 
        {
            return ;
        }
        else
        {
           
                shoot();
                prochainTir = Time.time + cadence;
            
        }

           
        
    }
    private void shoot()
    {
       //retire une cartouche
        qteCartouchesChargeur--;
        Debug.Log("feux");
        //si la routine est deja en cours
        if(routine != null)
        {
            //arrete la coroutine
            StopCoroutine(routine);

        }
        // on commence la routine du muzzleflash
        routine = StartCoroutine(FlashRoutine());
        Ray ray = new Ray(cameraFPS.transform.position,cameraFPS.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, portee, masqueTir)) 
        {
            if (hit.collider.CompareTag("ennemi")) 
            {
               controleEnnemis vie = hit.collider.GetComponent<controleEnnemis>();
                vie.PrendreDegats(dommage);
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * portee, Color.red, 1f);

    }
    */
}
