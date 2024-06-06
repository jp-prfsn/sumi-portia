using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopProgram : MonoBehaviour
{
    RectTransform rt;
    public AnimationCurve collapeCurve;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }
    
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void SetUp(DesktopFile file)
    {

    }

    public void Close()
    {
        DesktopManager.instance.aSource.Play();
        DesktopManager.instance.ReinstateCloseDesktopButton();
        this.gameObject.SetActive(false);
        
    }

    

    
}
