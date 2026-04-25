using UnityEngine;

[System.Serializable]
public class CardTemp
{
    public string cardName;
    public Sprite icon;

    public CardTemp(string name, Sprite icon)
    {
        this.cardName = name;
        this.icon = icon;
    }
}