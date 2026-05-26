using UnityEngine;
using UnityEngine.SceneManagement;
public class gestionAcenseur : MonoBehaviour
{
     [SerializeField] private int indexSceneSuivante = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        SceneManager.LoadScene(indexSceneSuivante);
    }
}
