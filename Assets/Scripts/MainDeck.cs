using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct TileEntry
{
    public Tile tile;
    public int count;
}

public class MainDeck : TileCollection
{
    [SerializeField] TileEntry[] levelTiles;
    [SerializeField] HandManager handManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] PlayerPathFinding e;
    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        LoadFromLevelData(levelTiles);

        for (int i = 0; i < 5; i++)
        {
            this.DealOneTo(handManager);
        }
    }
    public void FillHand()
    {
        if (handManager != null)
        {

            if (handManager.handCards.Count < handManager.maxCards) 
            {
                text.text = "";
                handManager.WashHandToDraw();

                for (int i = 0; i < 5; i++)
                {
                    this.DealOneTo(handManager);
                }
                e.StartPathFinding();
                gridManager.NextTurn();
            }
            else
            {
                
                text.text = "Must play at least one Tile!";
            }
        }
    }

    public void LoadFromLevelData(TileEntry[] entries)
    {
        Clear();
        foreach (TileEntry entry in entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Tile tile = entry.tile;
                if (tile != null)
                    Add(tile);
                else
                    Debug.LogWarning($"Prefab {entry.tile.name} has no Tile component.");
            }
        }

        // randomize the deck
        for (int i = 0; i < tiles.Count; i++)
        {
            int j = Random.Range(i, tiles.Count);
            Tile temp = tiles[i];
            tiles[i] = tiles[j];
            tiles[j] = temp;
        }
    }

    public void DealAllTo(TileCollection hand = null)
    {
        if (hand == null) hand = handManager;

        Tile[] snapshot = new Tile[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (Tile tile in snapshot)
            TransferTo(tile, hand);
    }

    public void DealOneTo(TileCollection hand = null)
    {
        if (hand == null) hand = handManager;

        if (tiles.Count == 0) return;
        TransferTo(tiles[0], hand);
    }
}
