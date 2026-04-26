using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandManager : TileCollection
{
    public int maxCards = 5;
    [SerializeField] CardDisplay selectedCard;
    [SerializeField] public List<CardDisplay> handCards = new();
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform handParent;

    TileBase selectedTileObj;
    [SerializeField] MainDeck mainDeck;
    [SerializeField] DiscardPile discardPile;

    [SerializeField] PlayerInput playerInput;
    private InputAction rightClickAction;

    void Start()
    {
        // Example cards for testing
        MaxCard = maxCards;
    }

    void Awake()
    {
        // hook right click action to deselect card
        rightClickAction = playerInput?.actions.FindActionMap("UI").FindAction("RightClick");
        if (rightClickAction != null)
        {
            rightClickAction.performed += ctx => DeselectCard();
            rightClickAction.Enable();
            Debug.Log("Right click has been enabled");
        }

    }

    // add a card to the hand, if the hand is not full
    protected override void OnTileAdded(Tile tile)
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
            TransferTo(card.tileData, discardPile);
            Destroy(card.gameObject);
        }
    }

    protected override void OnTileRemoved(Tile tile)
    {
        // future: notify UI to update hand display
    }

    // return all cards in hand to the main deck and clear the hand
    public void WashHandToDraw()
    {
        DeselectCard();
        foreach (CardDisplay card in handCards)
        {
            TransferTo(card.tileData, mainDeck);
            Destroy(card.gameObject);
        }
        handCards.Clear();
    }

    // clear the hand of all cards
    public void ClearHand()
    {
        foreach (CardDisplay card in handCards)
        {
            Destroy(card.gameObject);
        }
        handCards.Clear();
        Clear();
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
        selectedTileObj = tile;
        tile.Init(card.tileData, OnPlayCard);
        tile.SelectTile();
    }

    // deselect the currently selected card
    void DeselectCard(bool played = false)
    {
        selectedCard?.UnhoverCard();
        selectedCard = null;

        // return the tile to the pool id not played
        // if (!played)
        // {
        //     TilesObjPool.Instance.ReturnTile(selectedTileObj);
        // }

        selectedTileObj?.DeselectTile();
        // Destroy the tile if it wasn't played on the board
        if (!played && selectedTileObj != null) 
        {
            Destroy(selectedTileObj.gameObject);
            selectedTileObj = null;
            Debug.Log("Tile has been destroyed");
        }
        else
        {
            if (selectedTileObj?.tileData.isRotationCard == true)
            {
                TilesObjPool.Instance.ReturnTile(selectedTileObj);
            }
            selectedTileObj = null;
        }
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
        Debug.Log("Card played callback received with value: " + x);
        RemoveCard(selectedCard, played);
    }

    // set the selected card when a card is clicked
    public void OnCardClicked(CardDisplay card)
    {
        SelectCard(card);
    }

    // debug -------------
    [SerializeField] Tile debugCard;
    [ContextMenu("Add A Card")]
    void AddRandomCard()
    {
        Add(debugCard);
    }

}