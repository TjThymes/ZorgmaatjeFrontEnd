using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FoodManager : MonoBehaviour, IPoolable
{
    [SerializeField] List<Sprite> FoodSprite;

    public UnityEvent DeadZoneTouched;
    public UnityEvent FoodClicked;

    public float limitX = 2.5f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        MiniGame2Manager gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MiniGame2Manager>();
        DeadZoneTouched.AddListener(() => gameController.CollisionTriggered(this.gameObject));
        FoodClicked.AddListener(() => gameController.OnClickedTrigger(this.gameObject));
        limitX = 2.2f;
        this.gameObject.GetComponent<Button>().onClick.AddListener(onClickEvent);
    }

    private void Update()
    {
        if (Mathf.Abs(this.gameObject.transform.position.x) >= limitX)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Sign(pos.x) * limitX;
            transform.position = pos;

            Vector2 velocity = rb.linearVelocity;
            velocity.x = -velocity.x;
            rb.linearVelocity = velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            DeadZoneTouched.Invoke();
        }
    }

    private void onClickEvent()
    {
        FoodClicked?.Invoke();
    }

    public void OnSpawn()
    {
        this.gameObject.GetComponent<Image>().sprite = FoodSprite[Random.Range(0, FoodSprite.Count)];
    }

    public void OnReturnToPool()
    {
    }
}
