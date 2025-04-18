

public class DiaryNoteData
{
    public string title;
    public string fullNote;
    public string q2;
    public string q3;
    public string savedMood;
    public string savedAnswer2;
    public string savedAnswer3;
    public DiaryNoteData(string title, string fullNote, string q2, string q3, string savedMood, string savedAnswer2, string savedAnswer3)
    {
        this.title = title;
        this.fullNote = fullNote;
        this.q2 = q2;
        this.q3 = q3;
        this.savedMood = savedMood;
        this.savedAnswer2 = savedAnswer2;
        this.savedAnswer3 = savedAnswer3;
    }
}
