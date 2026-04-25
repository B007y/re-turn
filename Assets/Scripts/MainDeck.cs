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

    void Start()
    {
        LoadFromLevelData(levelTiles);
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
    }

    public void DealAllTo(TileCollection hand)
    {
        Tile[] snapshot = new Tile[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (Tile tile in snapshot)
            TransferTo(tile, hand);
    }

    public bool DealOneTo(TileCollection hand)
    {
        if (tiles.Count == 0) return false;
        TransferTo(tiles[0], hand);
        return true;
    }
}
