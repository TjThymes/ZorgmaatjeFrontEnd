using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiaryNoteManager : MonoBehaviour
{
    private List<DiaryNoteData> diaryNotes = new List<DiaryNoteData>();
    public GameObject noteSummaryPrefab;
    public Transform noteListParent;
    private DiaryNoteData currentNote;
    public GameObject diaryNoteObject;

    [SerializeField] private List<Sprite> feeling = new List<Sprite>();

    public void AddOrUpdateDiaryNote(string title, string fullNote, string q2, string q3, string savedMood, string savedAnswer2, string savedAnswer3)
    {
        // Check if the title is 'nieuwe notitie' and replace with current date
        if (title == "nieuwe notitie")
        {
            title = DateTime.Now.ToString("yyyy-MM-dd");
        }

        // Check if a note with the same title already exists
        DiaryNoteData existingNote = diaryNotes.Find(note => note.title == title);

        if (existingNote != null)
        {
            // Update existing note
            existingNote.fullNote = fullNote;
            existingNote.q2 = q2;
            existingNote.q3 = q3;
            existingNote.savedMood = savedMood;
            existingNote.savedAnswer2 = savedAnswer2;
            existingNote.savedAnswer3 = savedAnswer3;
            UpdateNoteSummaryButton(existingNote);
        }
        else
        {
            // Create new note and add to list
            DiaryNoteData newNote = new DiaryNoteData(title, fullNote, q2, q3, savedMood, savedAnswer2, savedAnswer3);
            diaryNotes.Add(newNote);
            Debug.Log($"Added new note: {title}");
            CreateNoteSummaryButton(newNote);
        }
    }

    private void CreateNoteSummaryButton(DiaryNoteData note)
    {
        GameObject newButton = Instantiate(noteSummaryPrefab, noteListParent);
        newButton.transform.SetAsFirstSibling();
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = note.title;
        Button buttonComponent = newButton.GetComponent<Button>();
        DiaryNoteTemplate diaryNoteTemplate = diaryNoteObject.GetComponent<DiaryNoteTemplate>();
        if (diaryNoteTemplate != null)
        {
            buttonComponent.onClick.AddListener(() => diaryNoteTemplate.AttachOpenButton(buttonComponent));
        }
        buttonComponent.onClick.AddListener(() => OpenDiaryNote(note.title));
        UpdateNoteSummaryButton(note);
    }

    private void UpdateNoteSummaryButton(DiaryNoteData note)
    {
        foreach (Transform child in noteListParent)
        {
            TextMeshProUGUI buttonText = child.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null && buttonText.text == note.title)
            {
                buttonText.text = note.title;
            }
            Image image = child.Find("Feeling").Find("Image").GetComponentInChildren<Image>();
            if (image != null)
            {
                switch (note.savedMood)
                {
                    case "Happy":
                        image.sprite = feeling[0]; break;
                    case "Neutral":
                        image.sprite = feeling[1]; break;
                    case "Sad":
                        image.sprite = feeling[2]; break;

                }
            }
        }
    }

    public void OpenDiaryNote(string title)
    {
        DiaryNoteData note = GetNoteByTitle(title);
        if (note != null)
        {
            currentNote = note;
        }
    }

    public DiaryNoteData GetCurrentNote()
    {
        return currentNote;
    }

    public DiaryNoteData GetNoteByTitle(string title)
    {
        return diaryNotes.Find(note => note.title == title);
    }

    public void PrintAllNotes()
    {
        foreach (var note in diaryNotes)
        {
            Debug.Log($"Title: {note.title}, Mood: {note.savedMood}");
        }
    }
}

