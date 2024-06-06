using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageviewProgram : DesktopProgram
{
    public UnityEngine.UI.Image image;
    public TMPro.TextMeshProUGUI filenameDisplay;

    public override void SetUp(DesktopFile file)
    {
        this.image.sprite = file.image;
        this.image.color = file.imageTint;
        this.filenameDisplay.text = file.fileName;
        this.Open();
    }
}
