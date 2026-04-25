using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class HandManager : MonoBehaviour
{
    int maxCards = 5;
    [SerializeField] CardDisplay selectedCard;
    [SerializeField] List<CardDisplay> handCards = new();
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform handParent;

    TileBase selectedTile;

    [SerializeField] PlayerInput playerInput;
    private InputAction rightClickAction;

    void Start()
    {
        // Example cards for testing
        TileData card1 = new TileData("Card1", null);
        TileData card2 = new TileData("Card2", null);

        AddCard(card1);
        AddCard(card2);

        // hook right click action to deselect card
        rightClickAction = playerInput?.actions.FindActionMap("UI").FindAction("RightClick");
        if (rightClickAction != null)
        {
            rightClickAction.performed += ctx => DeselectCard();
            rightClickAction.Enable();
        }
    }

    // add a card to the hand, if the hand is not full
    public void AddCard(TileData tile)
    {
        if (handCards.Count < maxCards)
        {
            GameObject newCard = Instantiate(cardPrefab, handParent);

            CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
            cardDisplay.SetCard(tile, OnCardClicked);

            handCards.Add(cardDisplay);
        }
        else
        {
            Debug.Log("Hand is full!");
        }
    }

    // remove a card from the hand
    public void RemoveCard(CardDisplay card, bool played = false)
    {
        if (handCards.Contains(card))
        {
            if (selectedCard == card)
                DeselectCard(played);

            handCards.Remove(card);
            Destroy(card.gameObject);
        }
    }

    // clear the hand of all cards
    public void ClearHand()
    {
        foreach (CardDisplay card in handCards)
        {
            Destroy(card.gameObject);
        }
        handCards.Clear();
    }

    // select a card, if the same card is selected again, play the card and deselect it
    // hover the card and ready to play it
    void SelectCard(CardDisplay card)
    {
        if (selectedCard == card)
        {
            return;
        }
        else if (selectedCard != null)
        {
            DeselectCard();
        }
        selectedCard = card;
        card.HoverCard();

        TileBase tile = TilesObjPool.Instance.GetTile();
        selectedTile = tile;
        tile.Init(card.tileData, OnPlayCard);
        tile.SelectTile();
    }

    // deselect the currently selected card
    void DeselectCard(bool played = false)
    {
        selectedCard?.UnhoverCard();
        selectedCard = null;

        // return the tile to the pool id not played
        if (!played)
        {
            TilesObjPool.Instance.ReturnTile(selectedTile);
        }
        selectedTile?.DeselectTile();
        selectedTile = null;
    }

    // play the selected card and remove it from the hand
    // public void OnPlayCard(CardDisplay card)
    // {
    //     // Implement card play logic here
    //     // card.card.Play();

    //     // assume all cards are tile
    //     RemoveCard(card);
    // }


    // callback -----------------
    // remove the card from hand
    public void OnPlayCard(int x)
    {
        // assume all cards are tile
        bool played = x == 0;
        RemoveCard(selectedCard, played);
    }

    // set the selected card when a card is clicked
    public void OnCardClicked(CardDisplay card)
    {
        SelectCard(card);
    }

}
public class TileData
{
    public string tileName;
    public Sprite tileSprite;
    public int[] openDirections;

    public TileData(string name, Sprite sprite)
    {
        this.tileName = name;
        this.tileSprite = sprite;
        this.openDirections = new int[] {1, 2};
    }
}
