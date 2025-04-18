using UnityEngine;

public class ICEButton : MonoBehaviour
{
    public GameObject ICEScreen;
    public void ToggleICEScreen()
    {
        ICEScreen.SetActive(!ICEScreen.activeInHierarchy);
    }
}
