using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DesktopFile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Desktop File can be either Text or Image
    public enum FileType
    {
        Text,
        Image
    }
    public FileType fileType;
    
    // Text File
    [TextArea(3, 10)]
    public string text;

    // Image File
    public Sprite image;

    // File Name
    public string fileName;

    public Sprite textFileIcon;
    public Sprite imageFileIcon;
    public Color imageTint = Color.white;

    private RectTransform rectTransform;

    bool firstClick = false;

    public AudioSource fileAudio;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        GetComponentInChildren<TextMeshProUGUI>().text = fileName;
        if (fileType == FileType.Text)
        {
            GetComponentInChildren<Image>().sprite = textFileIcon;
        }
        else if (fileType == FileType.Image)
        {
            GetComponentInChildren<Image>().sprite = imageFileIcon;
        }
    }

    void OpenFile()
    {
        if (fileType == FileType.Text)
        {
            DesktopManager.instance.OpenTextFile(this);
        }
        else if (fileType == FileType.Image)
        {
            DesktopManager.instance.OpenImageFile(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DesktopManager.instance.draggingFile == null){
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DesktopManager.instance.draggingFile = this;
        fileAudio.Play();
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        if(!firstClick)
        {
            firstClick = true;
            StartCoroutine(CheckForDoubleClick());
        }
        
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        DesktopManager.instance.draggingFile = null;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private IEnumerator CheckForDoubleClick()
    {
        Vector2 originalPosition = rectTransform.anchoredPosition;
        yield return null;
        float doubleClickTime = 0.3f;
        float time = 0;
        while (time < doubleClickTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                rectTransform.anchoredPosition = originalPosition;
                OpenFile();
                firstClick = false;
                yield break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        firstClick = false;
    }

    

   
    private void LateUpdate()
    {
        // If the file is being dragged
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject())
        {
            // If the file is being dragged
            if (DesktopManager.instance.draggingFile == this)
            {
                Vector2 vector;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    this.transform.parent.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    Camera.main,
                    out vector
                );

                // Lerp towards 'vector' position
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, vector, 0.3f);
            }
        }
    }


    private void OnDrawGizmos()
    {
        gameObject.name = fileName;
    }
}
