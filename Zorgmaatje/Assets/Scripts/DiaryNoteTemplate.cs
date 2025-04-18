using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DiaryNoteTemplate : MonoBehaviour
{
    [Header("Diary Note Elements")]
    public TextMeshProUGUI titleText;
    public TMP_InputField fullNoteText;
    public TextMeshProUGUI question1;
    public GameObject smileyPanel;
    public Button happySmiley;
    public Button neutralSmiley;
    public Button sadSmiley;
    private string moodAnswer;
    private string answerText1;
    private string answerText2;
    private string answerText3;
    private Button openButton;
    public TextMeshProUGUI question2;
    public TextMeshProUGUI question3;
    public TMP_InputField answer2;
    public TMP_InputField answer3;

    private DiaryNoteManager diaryManager;

    private void Awake()
    {
        diaryManager = FindObjectOfType<DiaryNoteManager>();
        if (diaryManager == null)
        {
            Debug.LogError("DiaryNoteManager not found!");
        }
    }
    public void Initialize(string title, string fullNote, string q2, string q3, string savedMood, string savedAnswer2, string savedAnswer3)
    {
        titleText.text = title;
        fullNoteText.text = fullNote;
        question1.text = "Hoe voel je je?";
        question2.text = q2;
        answer2.text = savedAnswer2;
        question3.text = q3;
        answer3.text = savedAnswer3;
        SetMood(savedMood);
    }

    private void SetMood(string mood)
    {
        moodAnswer = mood;
        happySmiley.GetComponent<Image>().color = mood == "Happy" ? Color.green : Color.white;
        neutralSmiley.GetComponent<Image>().color = mood == "Neutral" ? Color.yellow : Color.white;
        sadSmiley.GetComponent<Image>().color = mood == "Sad" ? Color.red : Color.white;
    }

    public void ChangeMood(string mood)
    {
        SetMood(mood);
    }

    public void OpenDiaryNote()
    {
        diaryManager.PrintAllNotes();
        DiaryNoteData currentNote = diaryManager.GetCurrentNote();
        this.Initialize(currentNote.title, currentNote.fullNote, currentNote.q2, currentNote.q3, currentNote.savedMood, currentNote.savedAnswer2, currentNote.savedAnswer3);
        this.gameObject.SetActive(true);
    }

    public void CloseDiaryNote()
    {
        SaveNote();
        this.Initialize("nieuwe notitie", "", "Wat ging er" + Environment.NewLine + "goed vandaag?", "Wat ging er" + Environment.NewLine + "minder goed" + Environment.NewLine + "vandaag?", "", "", "");
        this.gameObject.SetActive(false);
    }

    public void SaveNote()
    {
        diaryManager.AddOrUpdateDiaryNote(titleText.text, fullNoteText.text, question2.text, question3.text, moodAnswer, answer2.text, answer3.text);
    }

    public void AttachOpenButton(Button button)
    {
        openButton = button;
        if (openButton != null)
        {
            openButton.onClick.AddListener(() => OpenDiaryNote());
        }
    }
}

public class DiaryNoteSummary
{
    public string title;
    public string fullNote;

    public DiaryNoteSummary(string title, string fullNote)
    {
        this.title = title;
        this.fullNote = fullNote;
    }
}

