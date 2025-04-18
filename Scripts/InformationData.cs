

using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class InformationData
{
    public string title;
    public string firstText;
    public string secondText;
    public UnityEngine.UI.Image picture;

    public InformationData(string title, string firstText, string secondText, UnityEngine.UI.Image picture)
    {
        this.title = title;
        this.firstText = firstText;
        this.secondText = secondText;
        this.picture = picture;
    }
}
