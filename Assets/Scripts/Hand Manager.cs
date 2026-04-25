using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandManager : MonoBehaviour
{
    int maxCards = 5;
    [SerializeField] CardDisplay selectedCard;
    [SerializeField] List<CardDisplay> handCards = new();
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform handParent;

    [SerializeField] PlayerInput playerInput;
    private InputAction rightClickAction;

    void Start()
    {
        // Example cards for testing
        CardTemp card1 = new CardTemp("Card1", null);
        CardTemp card2 = new CardTemp("Card2", null);

        AddCard(card1);
        AddCard(card2);

        rightClickAction = playerInput?.actions.FindActionMap("UI").FindAction("RightClick");
        if (rightClickAction != null)
        {
            rightClickAction.performed += ctx => DeselectCard();
            rightClickAction.Enable();
        }
    }

    // add a card to the hand, if the hand is not full
    public void AddCard(CardTemp card)
    {
        if (handCards.Count < maxCards)
        {
            GameObject newCard = Instantiate(cardPrefab, handParent);

            CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
            cardDisplay.SetCard(card, OnCardClicked);

            handCards.Add(cardDisplay);
        }
        else
        {
            Debug.Log("Hand is full!");
        }
    }

    // remove a card from the hand
    public void RemoveCard(CardDisplay card)
    {
        if (handCards.Contains(card))
        {
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
            PlayCard(card);
            DeselectCard();
            return;
        }
        else if (selectedCard != null)
        {
            DeselectCard();
        }
        selectedCard = card;
        card.HoverCard();
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
    // set the selected card when a card is clicked
    public void OnCardClicked(CardDisplay card)
    {
        SelectCard(card);
    }
}
