using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
<<<<<<< HEAD
    public CardTemp card;
=======
    public Tile tileData;
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    System.Action<CardDisplay> onClickCallback;

    [Header("Display Settings")]
    bool isSelecting = false;
    float hoverPixel = 128f;
    float hoverScale = 1.5f;

<<<<<<< HEAD
    public void SetCard(CardTemp newCard, System.Action<CardDisplay> onClick = null)
    {
        card = newCard;
        onClickCallback = onClick;

        icon.sprite = card.icon;
        nameText.text = card.cardName;
=======
    public void SetCard(Tile newCard, System.Action<CardDisplay> onClick = null)
    {
        tileData = newCard;
        onClickCallback = onClick;

        icon.sprite = tileData.sprite;
        nameText.text = tileData.TileName;
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
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