using System.Collections.Generic;
using UnityEngine;

public abstract class TileCollection : MonoBehaviour
{
<<<<<<< HEAD
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
=======
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
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        if (tile == null) return false;
        bool removed = tiles.Remove(tile);
        if (removed) OnTileRemoved(tile);
        return removed;
    }

<<<<<<< HEAD
    public bool Contains(TileBase tile)
=======
    public bool Contains(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        return tiles.Contains(tile);
    }

<<<<<<< HEAD
    public void TransferTo(TileBase tile, TileCollection destination)
    {
        if (!Contains(tile)) return;
        if (destination == null) return;
        Remove(tile);
        destination.Add(tile);
=======
    public bool TransferTo(Tile tile, TileCollection destination)
    {
        if (!Contains(tile)) return false;
        if (destination == null) return false;
        if (!destination.Add(tile)) return false;
        
        Remove(tile);
        return true;
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    }

    public void Clear()
    {
        tiles.Clear();
    }

<<<<<<< HEAD
    protected virtual void OnTileAdded(TileBase tile) { }
    protected virtual void OnTileRemoved(TileBase tile) { }
=======
    protected virtual void OnTileAdded(Tile tile) { }
    protected virtual void OnTileRemoved(Tile tile) { }
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
}
