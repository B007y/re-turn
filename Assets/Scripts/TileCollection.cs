using System.Collections.Generic;
using UnityEngine;

public abstract class TileCollection : MonoBehaviour
{
    protected List<TileBase> tiles = new List<TileBase>();

    public int Count => tiles.Count;

    public IReadOnlyList<TileBase> Tiles => tiles.AsReadOnly();

    public void Add(TileBase tile)
    {
        if (tile == null) return;
        if (tiles.Contains(tile)) return;
        tiles.Add(tile);
        OnTileAdded(tile);
    }

    public bool Remove(TileBase tile)
    {
        if (tile == null) return false;
        bool removed = tiles.Remove(tile);
        if (removed) OnTileRemoved(tile);
        return removed;
    }

    public bool Contains(TileBase tile)
    {
        return tiles.Contains(tile);
    }

    public void TransferTo(TileBase tile, TileCollection destination)
    {
        if (!Contains(tile)) return;
        if (destination == null) return;
        Remove(tile);
        destination.Add(tile);
    }

    public void Clear()
    {
        tiles.Clear();
    }

    protected virtual void OnTileAdded(TileBase tile) { }
    protected virtual void OnTileRemoved(TileBase tile) { }
}
