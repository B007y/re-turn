<<<<<<< HEAD
=======
using Unity.VisualScripting;
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
using UnityEngine;

[System.Serializable]
public struct TileEntry
{
<<<<<<< HEAD
    public GameObject tilePrefab;
=======
    public Tile tile;
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    public int count;
}

public class MainDeck : TileCollection
{
    [SerializeField] TileEntry[] levelTiles;
<<<<<<< HEAD
    [SerializeField] Transform spawnParent;
=======
    [SerializeField] HandManager handManager;
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd

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
<<<<<<< HEAD
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
=======
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
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    }
}
