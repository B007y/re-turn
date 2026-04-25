using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public TileData tileData;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    System.Action<CardDisplay> onClickCallback;

    [Header("Display Settings")]
    bool isSelecting = false;
    float hoverPixel = 128f;
    float hoverScale = 1.5f;

    public void SetCard(TileData newCard, System.Action<CardDisplay> onClick = null)
    {
        tileData = newCard;
        onClickCallback = onClick;

        icon.sprite = tileData.tileSprite;
        nameText.text = tileData.tileName;
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