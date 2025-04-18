using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public Canvas canvas;
    public float Xmin = -2.7f;
    public float Xmax = 2.7f;
    private float timer = 0f;
    private float spawnInterval;

    void Start()
    {
        Xmax = canvas.scaleFactor * Xmax;
        Xmin = canvas.scaleFactor * Xmin;
        SetRandomSpawnInterval();
        SpawnBubble();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnBubble();
            timer = 0f;
            SetRandomSpawnInterval();
        }
    }

    void SetRandomSpawnInterval()
    {
        spawnInterval = Random.Range(1.5f, 3f);
    }

    public void SpawnBubble()
    {
        float randomX = Random.Range(Xmin, Xmax);
        Vector3 spawnPosition = new Vector3(randomX, -6f, 1f);
        var bubble = ObjectPoolManager.SpawnObject(ObjectPoolManager.PoolType.Bubble, spawnPosition, Quaternion.identity);

        SpriteRenderer spriteRenderer = bubble.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = bubble.AddComponent<SpriteRenderer>();
        }

        float randomScale = Random.Range(0.4f, 1f) * canvas.scaleFactor;
        bubble.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        Color bubbleColor = spriteRenderer.color;
        bubbleColor.a = 0.3f;
        spriteRenderer.color = bubbleColor;

        bubble.AddComponent<Bubblezz>();
    }
}
