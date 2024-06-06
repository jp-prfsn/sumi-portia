using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotepadProgram : DesktopProgram
{
    public TMPro.TextMeshProUGUI inputField;
    public RectTransform contentContainer;

    public override void SetUp(DesktopFile file)
    {
        inputField.text = $"[FILE: {file.fileName}]\n\n----------\n\nCONTENT: {file.text}";
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, inputField.preferredHeight + 30);
        Debug.Log(inputField.preferredHeight + 30);
        Open();
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup(){
        yield return new WaitForEndOfFrame();
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, inputField.preferredHeight + 30);
        Debug.Log(inputField.preferredHeight + 30);
    }
}
