using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UINode : MonoBehaviour
{
    public string nodeId;
    public TextMeshProUGUI titleText;
    public GameObject descriptionContainer;
    public TextMeshProUGUI descriptionText;
    public Button clickButton;

    private UIGraphManager graphManager;

    public void Initialize(string id, string title, string desc, UIGraphManager manager)
    {
        nodeId = id;
        titleText.text = title;
        descriptionText.text = desc;
        graphManager = manager;

        if (descriptionContainer != null)
        {
            descriptionContainer.SetActive(false);
        }

        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(() => graphManager.OnNodeClicked(title));
        }

        if (titleText != null)
        {
            AddClickHandlerTo(titleText.gameObject, () =>
            {
                if (descriptionText != null)
                {
                    descriptionContainer.gameObject.SetActive(!descriptionContainer.gameObject.activeSelf);
                }
            });
        }

        if (descriptionText != null)
        {
            AddClickHandlerTo(descriptionText.gameObject, () =>
            {
                if (descriptionText != null)
                {
                    descriptionContainer.gameObject.SetActive(!descriptionContainer.gameObject.activeSelf);
                }
            });
        }
    }

    private void AddClickHandlerTo(GameObject target, System.Action onClick)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }
        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => onClick());
        trigger.triggers.Add(entry);
    }
}
