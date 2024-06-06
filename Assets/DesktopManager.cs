using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopManager : MonoBehaviour
{
    public static DesktopManager instance;

    public NotepadProgram notepadProgram;
    public ImageviewProgram imageviewProgram;

    public DesktopFile draggingFile;

    public GameObject closeDesktopButton;

    public AudioSource aSource;


    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
        transform.parent.gameObject.SetActive(false);
    }

    public void OpenTextFile(DesktopFile file)
    {
        closeDesktopButton.SetActive(false);
        Debug.Log("Opening Text File: " + file.fileName);
        notepadProgram.SetUp(file);
        //notepadProgram.Open();
    }

    public void OpenImageFile(DesktopFile file)
    {
        closeDesktopButton.SetActive(false);
        Debug.Log("Opening Image File: " + file.fileName);
        imageviewProgram.SetUp(file);
        //imageviewProgram.Open();
    }

    


    

    public void ReinstateCloseDesktopButton()
    {
        closeDesktopButton.SetActive(true);
    }
}
