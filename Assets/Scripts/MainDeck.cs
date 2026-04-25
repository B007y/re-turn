using UnityEngine;

[System.Serializable]
public struct TileEntry
{
    public GameObject tilePrefab;
    public int count;
}

public class MainDeck : TileCollection
{
    [SerializeField] TileEntry[] levelTiles;
    [SerializeField] Transform spawnParent;

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
                GameObject obj = Instantiate(entry.tilePrefab, spawnParent);
                TileBase tile = obj.GetComponent<TileBase>();
                if (tile != null)
                    Add(tile);
                else
                    Debug.LogWarning($"Prefab {entry.tilePrefab.name} has no TileBase component.");
            }
        }
    }

    public void DealAllTo(PlayerHand hand)
    {
        TileBase[] snapshot = new TileBase[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (TileBase tile in snapshot)
            TransferTo(tile, hand);
    }

    public bool DealOneTo(PlayerHand hand)
    {
        if (tiles.Count == 0) return false;
        TransferTo(tiles[0], hand);
        return true;
    }
}
