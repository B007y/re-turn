using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

<<<<<<< HEAD
public class HandManager : MonoBehaviour
=======
public class HandManager : TileCollection
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
{
    int maxCards = 5;
    [SerializeField] CardDisplay selectedCard;
    [SerializeField] List<CardDisplay> handCards = new();
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform handParent;

<<<<<<< HEAD
=======
    TileBase selectedTileObj;
    [SerializeField] MainDeck mainDeck;
    [SerializeField] DiscardPile discardPile;

>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    [SerializeField] PlayerInput playerInput;
    private InputAction rightClickAction;

    void Start()
    {
        // Example cards for testing
<<<<<<< HEAD
        CardTemp card1 = new CardTemp("Card1", null);
        CardTemp card2 = new CardTemp("Card2", null);

        AddCard(card1);
        AddCard(card2);

=======
        MaxCard = maxCards;
        for (int i = 0; i < maxCards; i++)
        {
            mainDeck.DealOneTo(this);
        }

        // hook right click action to deselect card
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
        rightClickAction = playerInput?.actions.FindActionMap("UI").FindAction("RightClick");
        if (rightClickAction != null)
        {
            rightClickAction.performed += ctx => DeselectCard();
            rightClickAction.Enable();
        }
    }

    // add a card to the hand, if the hand is not full
<<<<<<< HEAD
    public void AddCard(CardTemp card)
=======
    protected override void OnTileAdded(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        if (handCards.Count < maxCards)
        {
            GameObject newCard = Instantiate(cardPrefab, handParent);

            CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
<<<<<<< HEAD
            cardDisplay.SetCard(card, OnCardClicked);
=======
            cardDisplay.SetCard(tile, OnCardClicked);
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd

            handCards.Add(cardDisplay);
        }
        else
        {
            Debug.Log("Hand is full!");
        }
    }

    // remove a card from the hand
<<<<<<< HEAD
    public void RemoveCard(CardDisplay card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
=======
    public void RemoveCard(CardDisplay card, bool played = false)
    {
        if (handCards.Contains(card))
        {
            if (selectedCard == card)
                DeselectCard(played);

            handCards.Remove(card);
            TransferTo(card.tileData, discardPile);
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
            Destroy(card.gameObject);
        }
    }

<<<<<<< HEAD
=======
    protected override void OnTileRemoved(Tile tile)
    {
        // future: notify UI to update hand display
    }

    // return all cards in hand to the main deck and clear the hand
    public void WashHandToDraw()
    {
        foreach (CardDisplay card in handCards)
        {
            TransferTo(card.tileData, mainDeck);
            Destroy(card.gameObject);
        }
        handCards.Clear();
    }

>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    // clear the hand of all cards
    public void ClearHand()
    {
        foreach (CardDisplay card in handCards)
        {
            Destroy(card.gameObject);
        }
        handCards.Clear();
<<<<<<< HEAD
=======
        Clear();
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    }

    // select a card, if the same card is selected again, play the card and deselect it
    // hover the card and ready to play it
    void SelectCard(CardDisplay card)
    {
        if (selectedCard == card)
        {
<<<<<<< HEAD
            PlayCard(card);
            DeselectCard();
=======
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
            return;
        }
        else if (selectedCard != null)
        {
            DeselectCard();
        }
        selectedCard = card;
        card.HoverCard();
<<<<<<< HEAD
    }

    // deselect the currently selected card
    void DeselectCard()
    {
        selectedCard?.UnhoverCard();
        selectedCard = null;
    }

    // play the selected card and remove it from the hand
    void PlayCard(CardDisplay card)
    {
        // Implement card play logic here
        // todo
        // card.card.Play();
        RemoveCard(card);
    }

    // callback -----------------
=======

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
        if (!played)
        {
            TilesObjPool.Instance.ReturnTile(selectedTileObj);
        }
        selectedTileObj?.DeselectTile();
        selectedTileObj = null;
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

>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    // set the selected card when a card is clicked
    public void OnCardClicked(CardDisplay card)
    {
        SelectCard(card);
    }
<<<<<<< HEAD
}
=======

}
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
