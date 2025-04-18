using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGame2Manager : MonoBehaviour
{
    public GameObject Heart1, Heart2, Heart3, GameOverScreen, VictoryScreen, DeadZone, Eten;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image ProgressBar;
    public float progress = 0.5f;
    public int Hearts = 3;
    public float spawnInterval = 5.0f;
    public bool gameOver = false;
    public bool gameWon = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var content = PlayerPrefs.GetString("SelectedContentID");
        if (content == "GezondEten") StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Eten.activeSelf == false) return;

        if (gameOver || gameWon) return;

        spawnInterval -= Time.deltaTime;
        if (spawnInterval <= 0)
        {
            SpawnFood();
            spawnInterval = 1.0f;
        }
    }

    private void StartGame()
    {
        Debug.Log(Eten.activeSelf);
        Eten.SetActive(true);
        Debug.Log(Eten.activeSelf);
        ProgressBar.fillAmount = progress;
        GameOverScreen.SetActive(false);
        VictoryScreen.SetActive(false);
        Debug.Log("Game Started");
    }

    void SpawnFood()
    {
        if (gameOver || gameWon) return;

        // Randomly choose between GFood, BFood, and Sugar
        ObjectPoolManager.PoolType foodToSpawn;
        float randomValue = Random.value;
        if (randomValue < 0.33f)
        {
            foodToSpawn = ObjectPoolManager.PoolType.GFood;
        }
        else if (randomValue < 0.66f)
        {
            foodToSpawn = ObjectPoolManager.PoolType.BFood;
        }
        else
        {
            foodToSpawn = ObjectPoolManager.PoolType.Sugar;
        }

        // Define the spawn area relative to the Canvas
        float minX = -300f;
        float maxX = 300f;
        float minY = 1000f;
        float maxY = 1300f;

        // Calculate a random spawn position within the defined area
        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);

        // Instantiate the food object as a child of the Canvas
        GameObject spawnedFood = ObjectPoolManager.SpawnObject(foodToSpawn, spawnPosition, Quaternion.identity);

        // Ensure the spawned food object has the correct components and settings
        RectTransform rectTransform = spawnedFood.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition3D = spawnPosition;
            rectTransform.localScale = Vector3.one;
        }

        Rigidbody2D rb = spawnedFood.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = spawnedFood.AddComponent<Rigidbody2D>();            
        }
        rb.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.1f, 0.1f));
        rb.gravityScale = Random.Range(0.01f, 0.3f);
        rb.mass = Random.Range(0.5f, 1.5f);
        rb.AddForce(transform.up * 1f, ForceMode2D.Impulse);
    }

    public void IncreaseProgress(GameObject caller)
    {
        var oldProgress = progress;
        progress += 0.1f;
        ProgressBar.fillAmount = Mathf.Lerp(oldProgress, progress, 1);
        if (progress >= 1)
        {
            GameWon();
        }
        ObjectPoolManager.ReturnObjectToPool(caller);
    }

    public void DecreaseProgress(GameObject caller)
    {
        var oldProgress = progress;
        progress -= 0.1f;
        ProgressBar.fillAmount = Mathf.Lerp(oldProgress, progress, 1);
        if (progress <= 0)
        {
            DecreaseHeart();
            progress += 0.5f;
        }
        ObjectPoolManager.ReturnObjectToPool(caller);
    }

    public void DecreaseHeart()
    {
        Hearts--;
        if (Hearts == 2)
        {
            Heart3.SetActive(false);
        }
        else if (Hearts == 1)
        {
            Heart2.SetActive(false);
        }
        else if (Hearts == 0)
        {
            Heart1.SetActive(false);
            GameOver();
        }
    }

    public void DecreaseHeart(GameObject caller)
    {
        DecreaseHeart();
        ObjectPoolManager.ReturnObjectToPool(caller);
    }

    public void GameOver()
    {
        gameOver = true;
        GameOverScreen.SetActive(true);
        Eten.SetActive(false);
        StartCoroutine(DelayAction(10f));
    }

    public void GameWon()
    {
        gameWon = true;
        Eten.SetActive(false);
        VictoryScreen.SetActive(true);
        StartCoroutine(DelayAction(10f));
    }

    IEnumerator DelayAction(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("HomeScene");
    }

    public void CollisionTriggered(GameObject touched)
    {
        ObjectInDeadZone(touched);
    }

    public void OnClickedTrigger(GameObject clicked)
    {
        OnObjectClicked(clicked);
    }

    private void OnObjectClicked(GameObject collision)
    {
        if (collision.gameObject.CompareTag("GFood"))
        {
            IncreaseProgress(collision);
        }
        else if (collision.gameObject.CompareTag("BFood"))
        {
            DecreaseProgress(collision);
        }
        else if (collision.CompareTag("Sugar"))
        {
            DecreaseProgress(collision);
            DecreaseProgress(collision);
        }
    }

    private void ObjectInDeadZone(GameObject collision)
    {
        // Check if the object entering the DeadZone is Sugar, GFood, or BFood
        if (collision.CompareTag("GFood"))
        {
            DecreaseProgress(collision);
        }
        else if (collision.CompareTag("Sugar") || collision.CompareTag("BFood"))
        {
            if (Random.Range(0, 3) == 0)
            {
                IncreaseProgress(collision);
            }
            else
            {
                ObjectPoolManager.ReturnObjectToPool(collision);
            }
        }
    }
}
