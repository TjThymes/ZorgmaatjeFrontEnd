using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneContentLoader : MonoBehaviour
{
    public GameObject[] contentPanels; // Assign all your content panels in the Inspector

    void Start()
    {
        string contentId = PlayerPrefs.GetString("SelectedContentID", "");
        string nodeId = PlayerPrefs.GetString("SelectedNodeID", "");

        foreach (GameObject panel in contentPanels)
        {
            panel.SetActive(panel.name == contentId); // only show the matching panel
        }
    }

    public void BackToGraph()
    {
        SceneManager.LoadScene("HomeScene");
    }
}

