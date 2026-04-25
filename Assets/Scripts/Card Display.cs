using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Tile tileData;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    System.Action<CardDisplay> onClickCallback;

    [Header("Display Settings")]
    bool isSelecting = false;
    float hoverPixel = 128f;
    float hoverScale = 1.5f;

    public void SetCard(Tile newCard, System.Action<CardDisplay> onClick = null)
    {
        tileData = newCard;
        onClickCallback = onClick;

        icon.sprite = tileData.sprite;
        nameText.text = tileData.TileName;
    }

    public void HoverCard()
    {
        isSelecting = true;
        OnCardHoverEnter();
    }

    public void UnhoverCard()
    {
        isSelecting = false;
        OnCardHoverExit();
    }

    // callback -------------------
    public void OnCardClicked()
    {
        onClickCallback?.Invoke(this);
    }

    public void OnCardHoverEnter()
    {
        if (isSelecting) return;

        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + hoverPixel);
        transform.localScale = Vector2.one * hoverScale;
    }

    public void OnCardHoverExit()
    {
        if (isSelecting) return;

        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - hoverPixel);
        transform.localScale = Vector2.one;
    }
}