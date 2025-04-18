using System.Collections;
using UnityEngine;

public class Bubblezz : MonoBehaviour, IPoolable
{
    public float minSpeed = 0.3f;
    public float maxSpeed = 0.7f;
    public float fadeDuration = 20f;

    private SpriteRenderer spriteRenderer;
    private float speed;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > Camera.main.orthographicSize + 2f)
        {
            ObjectPoolManager.ReturnObjectToPool(this.gameObject);
        }
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = spriteRenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = spriteRenderer.color;
            spriteRenderer.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }
        ObjectPoolManager.ReturnObjectToPool(this.gameObject);
    }

    public void OnSpawn()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        speed = Random.Range(minSpeed, maxSpeed);
        StartCoroutine(FadeOut());
    }

    public void OnReturnToPool()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}

