using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameSystem : MonoBehaviour
{

    private float glucoseLevel = 6f;
    private float maxGlucoseLevel = 10f; 
    private float minGlucoseLevel = 4f; 
    private float eatingAmount = 1f; 
    private float bikingAmount = -.5f; 
    private int bikeProgression = 0;
    private int bikeProgressionPerClick = 10;
    private int maxBikeProgression = 100;
    private bool canPlay = true;
    private bool isBiking = false; 
    public RectTransform bikeRect;
    public RectTransform backgroundBarRect,progressionBarRect; 
    public GameObject GlucoseLevelText,ProgressionBackground,ProgressionBar,ButtonKeepBiking,ButtonEat, fietsen;

    void Start()
    {
        var content = PlayerPrefs.GetString("SelectedContentID");
        if (content == "Fietsen") StartGame();
    }

    void Update()
    {
        // if (isBiking)
        // {
        //     DecreaseGlucose(bikingDecreaseRate * Time.deltaTime);
        // }

        // if (glucoseLevel >= maxGlucoseLevel)
        // {
        //     glucoseLevel = maxGlucoseLevel;
        //     UpdateGlucoseText();
        //     Debug.Log("Game Over: You reached max glucose!");
        // }

    }

    private void StartGame()
    {
        glucoseLevel = 6f;
        bikeProgression = 0;
        canPlay = true;
        fietsen.SetActive(true);
        UpdateGlucoseText();
        MoveBike();
        ResizeBar();
    }
    public void EndMinigame()
    {
        canPlay = false;
        fietsen.SetActive(false);
        SceneManager.LoadScene("HomeScene");
    }
    public void Eat()
    {
        if (!canPlay) {return;}
        ChangeGlucose(eatingAmount);
    }
    public void Bike()
    {
        if (!canPlay) {return;}
        bikeProgression += bikeProgressionPerClick;
        if (bikeProgression > maxBikeProgression)
        {
            EndMinigame();
            return;
        }
        ResizeBar();
        MoveBike();
        ChangeGlucose(bikingAmount);
    }
    public void StartBiking()
    {
        isBiking = true;
    }

    public void StopBiking()
    {
        isBiking = false;
    }
    private void ChangeGlucose(float amount)
    {
        glucoseLevel += amount;
        if (glucoseLevel < minGlucoseLevel || glucoseLevel > maxGlucoseLevel)
        {
            StartGame();
            return;
        }
        UpdateGlucoseText();
    }

    private void UpdateGlucoseText()
    {
        GlucoseLevelText.GetComponent<TMP_Text>().text = "Glucose: " + glucoseLevel.ToString("G");
    }
    private void ResizeBar()
    {
        float percentage = (float)bikeProgression / maxBikeProgression;
        float backgroundWidth = backgroundBarRect.rect.width;
        float newWidth = (percentage) * backgroundWidth;
        progressionBarRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

    }
     // Assign your bike UI image here

    private void MoveBike()
    {
        float percentage = (float)bikeProgression / maxBikeProgression;
        float backgroundWidth = backgroundBarRect.rect.width;

        float newX = (backgroundWidth * percentage);

        Vector3 newPosition = new Vector3(newX - 540, 0, 0);

        bikeRect.localPosition = newPosition;
    }
}
