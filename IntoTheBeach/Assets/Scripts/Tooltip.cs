using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI heading,text;
    [SerializeField] Image image;
    public CanvasGroup canvasGroup;
    RectTransform rectTransform;
    [SerializeField] float offsetMult  = 1f;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetPosition();
    }

    public void SetContent(TooltipContent content)
    {
        if (heading != null)
        {
            if (content.heading != null)
                heading.text = content.heading;
            else heading.text = "";
        }
        if (text != null)
        {
            if (content.text != null)
                text.text = content.text;
            else text.text = "";
        }
        if (image != null)
        {
            if (content.sprite != null)
                image.sprite = content.sprite;
            else image.sprite = null;
        }
    }
    void SetPosition()
    {

        Vector2 mouse = Input.mousePosition;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouse);
        worldPos.z = 0f;

        transform.position = worldPos;

        float pivotX = (mouse.x / Screen.width - 0.5f) * 2f;
        float pivotY = (mouse.y / Screen.height - 0.5f) * 2f;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
    }
}

public struct TooltipContent
{
    public string heading, text;
    public Sprite sprite;
}