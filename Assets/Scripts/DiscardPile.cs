using UnityEngine;

public class DiscardPile : TileCollection
{
    public bool IsEmpty => tiles.Count == 0;

    public void RestoreAllTo(TileCollection destination)
    {
<<<<<<< HEAD
        TileBase[] snapshot = new TileBase[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (TileBase tile in snapshot)
            TransferTo(tile, destination);
    }

    protected override void OnTileAdded(TileBase tile)
=======
        Tile[] snapshot = new Tile[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (Tile tile in snapshot)
            TransferTo(tile, destination);
    }

    protected override void OnTileAdded(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        // future: play discard animation / sound
    }
}
