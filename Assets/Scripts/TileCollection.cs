using System.Collections.Generic;
using UnityEngine;

public abstract class TileCollection : MonoBehaviour
{
    protected List<Tile> tiles = new List<Tile>();

    public int Count => tiles.Count;
    public int MaxCard = -1;

    public IReadOnlyList<Tile> Tiles => tiles.AsReadOnly();

    public bool Add(Tile tile)
    {
        if (tile == null) return false;
        // if (tiles.Contains(tile)) return;
        if (MaxCard > 0 && tiles.Count >= MaxCard)
        {
            Debug.LogWarning("Cannot add tile: collection is full.");
            return false;
        }
        tiles.Add(tile);
        OnTileAdded(tile);
        return true;
    }

    public bool Remove(Tile tile)
    {
        if (tile == null) return false;
        bool removed = tiles.Remove(tile);
        if (removed) OnTileRemoved(tile);
        return removed;
    }

    public bool Contains(Tile tile)
    {
        return tiles.Contains(tile);
    }

    public bool TransferTo(Tile tile, TileCollection destination)
    {
        if (!Contains(tile)) return false;
        if (destination == null) return false;
        if (!destination.Add(tile)) return false;
        
        Remove(tile);
        return true;
    }

    public void Clear()
    {
        tiles.Clear();
    }

    protected virtual void OnTileAdded(Tile tile) { }
    protected virtual void OnTileRemoved(Tile tile) { }
}
