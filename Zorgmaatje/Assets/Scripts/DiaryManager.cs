using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    [Header("References")]
    public GameObject summaryNotePrefab;
    public GameObject diaryNote;
    public Transform contentPanel;
    public Button addNoteButton;
    public DiaryNoteTemplate diaryNoteTemplate;

    void Start()
    {
        if (addNoteButton != null)
        {
            addNoteButton.onClick.AddListener(CreateNewNote);
        }
    }

    private void CreateNewNote()
    {
        if (summaryNotePrefab != null && contentPanel != null)
        {
            diaryNote.SetActive(true);
        }
        else
        {
            Debug.LogError("Noteprefab or contentPanel is not set in the inspector");
        }
    }
}
