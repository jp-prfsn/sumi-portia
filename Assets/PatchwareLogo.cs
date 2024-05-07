using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchwareLogo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Load Scene called instructions
        Invoke("LoadScene", 3);
        
    }

    void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions");
    }
}
